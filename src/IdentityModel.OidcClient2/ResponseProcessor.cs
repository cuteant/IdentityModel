﻿using CuteAnt.IdentityModel.Client;
using CuteAnt.IdentityModel.OidcClient.Infrastructure;
using CuteAnt.IdentityModel.OidcClient.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CuteAnt.IdentityModel.OidcClient
{
    internal class ResponseProcessor
    {
        private readonly OidcClientOptions _options;
        private TokenClient _tokenClient;
        private ILogger<ResponseProcessor> _logger;
        private readonly IdentityTokenValidator _tokenValidator;
        private readonly CryptoHelper _crypto;

        public ResponseProcessor(OidcClientOptions options)
        {
            _options = options;
            _logger = options.LoggerFactory.CreateLogger<ResponseProcessor>();

            _tokenValidator = new IdentityTokenValidator(options);
            _crypto = new CryptoHelper(options);
        }

        public async Task<ResponseValidationResult> ProcessResponseAsync(AuthorizeResponse authorizeResponse, AuthorizeState state)
        {
            _logger.LogTrace("ProcessResponseAsync");

            //////////////////////////////////////////////////////
            // validate common front-channel parameters
            //////////////////////////////////////////////////////

            if (string.IsNullOrEmpty(authorizeResponse.Code))
            {
                var error = "Missing authorization code";
                _logger.LogError(error);

                return new ResponseValidationResult { Error = error };
            }

            if (string.IsNullOrEmpty(authorizeResponse.State))
            {
                var error = "Missing state";
                _logger.LogError(error);

                return new ResponseValidationResult { Error = error };
            }

            if (!string.Equals(state.State, authorizeResponse.State, StringComparison.Ordinal))
            {
                var error = "Invalid state";
                _logger.LogError(error);

                return new ResponseValidationResult { Error = error };
            }

            switch (_options.Flow)
            {
                case OidcClientOptions.AuthenticationFlow.AuthorizationCode:
                    return await ProcessCodeFlowResponseAsync(authorizeResponse, state);
                case OidcClientOptions.AuthenticationFlow.Hybrid:
                    return await ProcessHybridFlowResponseAsync(authorizeResponse, state);
                default:
                    throw new ArgumentOutOfRangeException(nameof(_options.Flow), "Invalid authentication style");
            }
        }

        public async Task<ResponseValidationResult> ProcessHybridFlowResponseAsync(AuthorizeResponse authorizeResponse, AuthorizeState state)
        {
            _logger.LogTrace("ProcessHybridFlowResponseAsync");

            var result = new ResponseValidationResult();

            //////////////////////////////////////////////////////
            // validate front-channel response
            //////////////////////////////////////////////////////

            // id_token must be present
            if (authorizeResponse.IdentityToken.IsMissing())
            {
                result.Error = "Missing identity token.";
                _logger.LogError(result.Error);

                return result;
            }

            // id_token must be valid
            var frontChannelValidationResult = _tokenValidator.Validate(authorizeResponse.IdentityToken);
            if (frontChannelValidationResult.IsError)
            {
                result.Error = frontChannelValidationResult.Error ?? "Identity token validation error.";
                _logger.LogError(result.Error);

                return result;
            }

            // validate sub
            if (_options.Policy.RequireSubject)
            {
                var sub = frontChannelValidationResult.User.FindFirst(JwtClaimTypes.Subject);
                if (sub == null)
                {
                    return new ResponseValidationResult
                    {
                        Error = "sub is missing."
                    };
                }
            }

            // nonce must be valid
            if (!ValidateNonce(state.Nonce, frontChannelValidationResult.User))
            {
                result.Error = "Invalid nonce.";
                _logger.LogError(result.Error);

                return result;
            }

            // validate c_hash
            var cHash = frontChannelValidationResult.User.FindFirst(JwtClaimTypes.AuthorizationCodeHash);
            if (cHash == null)
            {
                if (_options.Policy.RequireAuthorizationCodeHash)
                {
                    return new ResponseValidationResult
                    {
                        Error = "c_hash is missing."
                    };
                }
            }
            else
            {
                if (!_crypto.ValidateHash(authorizeResponse.Code, cHash.Value, frontChannelValidationResult.SignatureAlgorithm))
                {
                    result.Error = "Invalid c_hash.";
                    _logger.LogError(result.Error);

                    return result;
                }
            }

            //////////////////////////////////////////////////////
            // process back-channel response
            //////////////////////////////////////////////////////

            // redeem code for tokens
            var tokenResponse = await RedeemCodeAsync(authorizeResponse.Code, state);
            if (tokenResponse.IsError)
            {
                _logger.LogError(tokenResponse.Error);
                result.Error = tokenResponse.Error;

                return result;
            }

            // validate token response
            var tokenResponseValidationResult = ValidateTokenResponse(tokenResponse);
            if (tokenResponseValidationResult.IsError)
            {
                result.Error = tokenResponseValidationResult.Error;
                return result;
            }

            // validate sub
            if (_options.Policy.RequireSubject)
            {
                var sub = tokenResponseValidationResult.IdentityTokenValidationResult.User.FindFirst(JwtClaimTypes.Subject);
                if (sub == null)
                {
                    return new ResponseValidationResult
                    {
                        Error = "sub is missing."
                    };
                }
            }

            // compare front & back channel subs
            var frontChannelSub = frontChannelValidationResult.User.FindFirst(JwtClaimTypes.Subject)?.Value ?? "none";
            var backChannelSub = tokenResponseValidationResult.IdentityTokenValidationResult.User.FindFirst(JwtClaimTypes.Subject)?.Value ?? "none";

            if (!string.Equals(frontChannelSub, backChannelSub, StringComparison.Ordinal))
            {
                var error = $"Subject on front-channel ({frontChannelSub}) does not match subject on back-channel ({backChannelSub}).";

                _logger.LogError(error);
                result.Error = error;

                return result;
            }

            return new ResponseValidationResult
            {
                AuthorizeResponse = authorizeResponse,
                TokenResponse = tokenResponse,
                User = tokenResponseValidationResult.IdentityTokenValidationResult.User
            };
        }

        public async Task<ResponseValidationResult> ProcessCodeFlowResponseAsync(AuthorizeResponse authorizeResponse, AuthorizeState state)
        {
            _logger.LogTrace("ProcessCodeFlowResponseAsync");

            var result = new ResponseValidationResult();

            //////////////////////////////////////////////////////
            // process back-channel response
            //////////////////////////////////////////////////////

            // redeem code for tokens
            var tokenResponse = await RedeemCodeAsync(authorizeResponse.Code, state);
            if (tokenResponse.IsError)
            {
                var error = $"Error redeeming code: {tokenResponse.Error}";
                _logger.LogError(error);

                result.Error = error;
                return result;
            }

            // validate token response
            var tokenResponseValidationResult = ValidateTokenResponse(tokenResponse);
            if (tokenResponseValidationResult.IsError)
            {
                var error = $"Error validating token response: {tokenResponseValidationResult.Error}";
                _logger.LogError(error);

                result.Error = error;
                return result;
            }

            return new ResponseValidationResult
            {
                AuthorizeResponse = authorizeResponse,
                TokenResponse = tokenResponse,
                User = tokenResponseValidationResult.IdentityTokenValidationResult.User
            };
        }

        public TokenResponseValidationResult ValidateTokenResponse(TokenResponse response, bool requireIdentityToken = true)
        {
            _logger.LogTrace("ValidateTokenResponse");

            var result = new TokenResponseValidationResult();

            // token response must contain an access token
            if (response.AccessToken.IsMissing())
            {
                result.Error = "Access token is missing on token response.";
                _logger.LogError(result.Error);

                return result;
            }

            if (requireIdentityToken)
            {
                // token response must contain an identity token (openid scope is mandatory)
                if (response.IdentityToken.IsMissing())
                {
                    result.Error = "Identity token is missing on token response.";
                    _logger.LogError(result.Error);

                    return result;
                }
            }

            if (response.IdentityToken.IsPresent())
            {
                // if identity token is present, it must be valid
                var validationResult = _tokenValidator.Validate(response.IdentityToken);
                if (validationResult.IsError)
                {
                    result.Error = validationResult.Error ?? "Identity token validation error";
                    _logger.LogError(result.Error);

                    return result;
                }

                // validate sub
                if (_options.Policy.RequireSubject)
                {
                    var sub = validationResult.User.FindFirst(JwtClaimTypes.Subject);
                    if (sub == null)
                    {
                        return new TokenResponseValidationResult
                        {
                            Error = "sub is missing."
                        };
                    }
                }

                // validate at_hash
                var atHash = validationResult.User.FindFirst(JwtClaimTypes.AccessTokenHash);
                if (atHash == null)
                {
                    if (_options.Policy.RequireAccessTokenHash)
                    {
                        return new TokenResponseValidationResult
                        {
                            Error = "at_hash is missing."
                        };
                    }
                }
                else
                {
                    if (!_crypto.ValidateHash(response.AccessToken, atHash.Value, validationResult.SignatureAlgorithm))
                    {
                        result.Error = "Invalid access token hash.";
                        _logger.LogError(result.Error);

                        return result;
                    }
                }
                
                return new TokenResponseValidationResult
                {
                    IdentityTokenValidationResult = validationResult
                };
            }

            return new TokenResponseValidationResult();
        }

        private bool ValidateNonce(string nonce, ClaimsPrincipal user)
        {
            _logger.LogTrace("ValidateNonce");

            var tokenNonce = user.FindFirst(JwtClaimTypes.Nonce)?.Value ?? "";
            var match = string.Equals(nonce, tokenNonce, StringComparison.Ordinal);

            if (!match)
            {
                _logger.LogError($"nonce ({nonce}) does not match nonce from token ({tokenNonce})");
            }

            return match;
        }

        private async Task<TokenResponse> RedeemCodeAsync(string code, AuthorizeState state)
        {
            _logger.LogTrace("RedeemCodeAsync");

            var client = GetTokenClient();

            var tokenResult = await client.RequestAuthorizationCodeAsync(
                code,
                state.RedirectUri,
                codeVerifier: state.CodeVerifier);

            return tokenResult;
        }

        private TokenClient GetTokenClient()
        {
            if (_tokenClient == null)
            {
                _tokenClient = TokenClientFactory.Create(_options);
            }

            return _tokenClient;
        }
    }
}