﻿using CuteAnt.IdentityModel.OidcClient.Results;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CuteAnt.IdentityModel.OidcClient
{
    internal class IdentityTokenValidator
    {
        private readonly ILogger _logger;
        private readonly OidcClientOptions _options;

        public IdentityTokenValidator(OidcClientOptions options)
        {
            _options = options;
            _logger = options.LoggerFactory.CreateLogger<IdentityTokenValidator>();
        }

        /// <summary>
        /// Validates the specified identity token.
        /// </summary>
        /// <param name="identityToken">The identity token.</param>
        /// <returns>The validation result</returns>
        public IdentityTokenValidationResult Validate(string identityToken)
        {
            _logger.LogTrace("Validate");

            var keys = new List<SecurityKey>();
            foreach (var webKey in _options.ProviderInformation.KeySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n });
                key.KeyId = webKey.Kid;

                keys.Add(key);

                _logger.LogDebug("Added signing key with kid: {kid}", key?.KeyId ?? "not set");
            }

            var parameters = new TokenValidationParameters
            {
                ValidIssuer = _options.ProviderInformation.IssuerName,
                ValidAudience = _options.ClientId,
                IssuerSigningKeys = keys,

                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role,

                ClockSkew = _options.ClockSkew
            };

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            SecurityToken token;
            ClaimsPrincipal user;

            try
            {
                user = handler.ValidateToken(identityToken, parameters, out token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return new IdentityTokenValidationResult
                {
                    Error = $"Error validating identity token: {ex.ToString()}"
                };
            }

            var jwt = token as JwtSecurityToken;
            var algorithm = jwt.Header.Alg;

            if (!_options.Policy.ValidSignatureAlgorithms.Contains(algorithm))
            {
                return new IdentityTokenValidationResult
                {
                    Error = $"Identity token uses invalid algorithm: {algorithm}"
                };
            };

            return new IdentityTokenValidationResult
            {
                User = user,
                SignatureAlgorithm = algorithm
            };
        }
    }
}