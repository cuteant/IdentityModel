using IdentityModel.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using PCLCrypto;
using static PCLCrypto.WinRTCrypto;
using IdentityModel.OidcClient.Infrastructure;
#if NET40
using CuteAnt.Extensions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace IdentityModel.OidcClient
{
  internal class ResponseValidator
  {
    private static readonly ILogger s_logger = TraceLogger.GetLogger<ResponseValidator>();

    private readonly OidcClientOptions _options;
    private TokenClient _tokenClient;

    public ResponseValidator(OidcClientOptions options)
    {
      _options = options;
    }

    public async Task<ResponseValidationResult> ValidateHybridFlowResponseAsync(AuthorizeResponse authorizeResponse, AuthorizeState state)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Validate hybrid flow response");
      var result = new ResponseValidationResult();

      //////////////////////////////////////////////////////
      // validate front-channel response
      //////////////////////////////////////////////////////

      // id_token must be present
      if (authorizeResponse.IdentityToken.IsMissing())
      {
        result.Error = "Missing identity token";
        s_logger.LogError(result.Error);

        return result;
      }

      // id_token must be valid
      var validationResult = await ValidateIdentityTokenAsync(authorizeResponse.IdentityToken);
      if (!validationResult.Success)
      {
        result.Error = validationResult.Error ?? "Identity token validation error";
        s_logger.LogError(result.Error);

        return result;
      }

      // nonce must be valid
      if (!ValidateNonce(state.Nonce, validationResult.Claims))
      {
        result.Error = "Invalid nonce";
        s_logger.LogError(result.Error);

        return result;
      }

      // if c_hash is present, it must be valid
      var signingAlgorithmBits = int.Parse(validationResult.SignatureAlgorithm.Substring(2));
      if (!ValidateAuthorizationCodeHash(authorizeResponse.Code, signingAlgorithmBits, validationResult.Claims))
      {
        result.Error = "Invalid c_hash";
        s_logger.LogError(result.Error);

        return result;
      }

      //////////////////////////////////////////////////////
      // process back-channel response
      //////////////////////////////////////////////////////

      // redeem code for tokens
      var tokenResponse = await RedeemCodeAsync(authorizeResponse.Code, state);
      if (tokenResponse.IsError || tokenResponse.IsHttpError)
      {
        s_logger.LogError(result.Error);
        result.Error = tokenResponse.Error;
        return result;
      }

      // validate token response
      var tokenResponseValidationResult = await ValidateTokenResponse(tokenResponse);
      if (!tokenResponseValidationResult.Success)
      {
        result.Error = tokenResponseValidationResult.Error;
        return result;
      }

      return new ResponseValidationResult
      {
        AuthorizeResponse = authorizeResponse,
        TokenResponse = tokenResponse,
        Claims = tokenResponseValidationResult.IdentityTokenValidationResult.Claims
      };
    }

    public async Task<ResponseValidationResult> ValidateCodeFlowResponseAsync(AuthorizeResponse authorizeResponse, AuthorizeState state)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Validate code flow response");
      var result = new ResponseValidationResult();

      //////////////////////////////////////////////////////
      // validate front-channel response
      //////////////////////////////////////////////////////

      // code must be present
      if (authorizeResponse.Code.IsMissing())
      {
        result.Error = "code is missing";
        s_logger.LogError(result.Error);

        return result;
      }

      if (!string.Equals(authorizeResponse.State, state.State, StringComparison.Ordinal))
      {
        result.Error = "invalid state";
        s_logger.LogError(result.Error);

        return result;
      }

      //////////////////////////////////////////////////////
      // process back-channel response
      //////////////////////////////////////////////////////

      // redeem code for tokens
      var tokenResponse = await RedeemCodeAsync(authorizeResponse.Code, state);
      if (tokenResponse.IsError || tokenResponse.IsHttpError)
      {
        result.Error = tokenResponse.Error;
        return result;
      }

      // validate token response
      var tokenResponseValidationResult = await ValidateTokenResponse(tokenResponse);
      if (!tokenResponseValidationResult.Success)
      {
        result.Error = tokenResponseValidationResult.Error;
        return result;
      }

      return new ResponseValidationResult
      {
        AuthorizeResponse = authorizeResponse,
        TokenResponse = tokenResponse,
        Claims = tokenResponseValidationResult.IdentityTokenValidationResult.Claims
      };
    }

    public async Task<TokenResponseValidationResult> ValidateTokenResponse(TokenResponse response, bool requireIdentityToken = true)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Validating token response");
      var result = new TokenResponseValidationResult();

      // token response must contain an access token
      if (response.AccessToken.IsMissing())
      {
        result.Error = "access token is missing on token response";
        s_logger.LogError(result.Error);

        return result;
      }

      if (requireIdentityToken)
      {
        // token response must contain an identity token (openid scope is mandatory)
        if (response.IdentityToken.IsMissing())
        {
          result.Error = "identity token is missing on token response";
          s_logger.LogError(result.Error);

          return result;
        }
      }

      if (response.IdentityToken.IsPresent())
      {
        // if identity token is present, it must be valid
        var validationResult = await ValidateIdentityTokenAsync(response.IdentityToken);
        if (!validationResult.Success)
        {
          result.Error = validationResult.Error ?? "Identity token validation error";
          s_logger.LogError(result.Error);

          return result;
        }

        // if at_hash is present, it must be valid
        var signingAlgorithmBits = int.Parse(validationResult.SignatureAlgorithm.Substring(2));
        if (!ValidateAccessTokenHash(response.AccessToken, signingAlgorithmBits, validationResult.Claims))
        {
          result.Error = "Invalid access token hash";
          s_logger.LogError(result.Error);

          return result;
        }

        return new TokenResponseValidationResult
        {
          IdentityTokenValidationResult = validationResult
        };
      }

      return new TokenResponseValidationResult();
    }

    private async Task<IdentityTokenValidationResult> ValidateIdentityTokenAsync(string idToken)
    {
      var providerInfo = await _options.GetProviderInformationAsync();

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Calling identity token validator: " + _options.IdentityTokenValidator.GetType().FullName);
      var validationResult = await _options.IdentityTokenValidator.ValidateAsync(idToken, _options.ClientId, providerInfo);

      if (validationResult.Success == false)
      {
        return validationResult;
      }

      var claims = validationResult.Claims;

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("identity token validation claims:");
      s_logger.LogClaims(claims);

      // validate audience
      var audience = claims.FindFirst(JwtClaimTypes.Audience)?.Value ?? "";
      if (!string.Equals(_options.ClientId, audience, StringComparison.Ordinal))
      {
        s_logger.LogError($"client id ({_options.ClientId}) does not match audience ({audience})");

        return new IdentityTokenValidationResult
        {
          Error = "invalid audience"
        };
      }

      // validate issuer
      var issuer = claims.FindFirst(JwtClaimTypes.Issuer)?.Value ?? "";
      if (!string.Equals(providerInfo.IssuerName, issuer, StringComparison.Ordinal))
      {
        s_logger.LogError($"configured issuer ({providerInfo.IssuerName}) does not match token issuer ({issuer}");

        return new IdentityTokenValidationResult
        {
          Error = "invalid issuer"
        };
      }

      return validationResult;
    }

    private bool ValidateNonce(string nonce, Claims claims)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("validate nonce");

      var tokenNonce = claims.FindFirst(JwtClaimTypes.Nonce)?.Value ?? "";
      var match = string.Equals(nonce, tokenNonce, StringComparison.Ordinal);

      if (!match)
      {
        s_logger.LogError($"nonce ({nonce}) does not match nonce from token ({tokenNonce})");
      }

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("success");
      return match;
    }

    private bool ValidateAuthorizationCodeHash(string code, int signingAlgorithmBits, Claims claims)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("validate authorization code hash");

      var cHash = claims.FindFirst(JwtClaimTypes.AuthorizationCodeHash)?.Value ?? "";
      if (cHash.IsMissing())
      {
        return true;
      }

      var hashAlgorithm = GetHashAlgorithm(signingAlgorithmBits);
      if (hashAlgorithm == null)
      {
        s_logger.LogError("No appropriate hashing algorithm found.");
      }

      var codeHash = hashAlgorithm.HashData(
          CryptographicBuffer.CreateFromByteArray(
              Encoding.UTF8.GetBytes(code)));

      byte[] codeHashArray;
      CryptographicBuffer.CopyToByteArray(codeHash, out codeHashArray);

      byte[] leftPart = new byte[signingAlgorithmBits / 16];
      Array.Copy(codeHashArray, leftPart, signingAlgorithmBits / 16);

      var leftPartB64 = Base64Url.Encode(leftPart);
      var match = leftPartB64.Equals(cHash);

      if (!match)
      {
        s_logger.LogError($"code hash ({leftPartB64}) does not match c_hash from token ({cHash})");
      }

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("success");
      return match;
    }

    private bool ValidateAccessTokenHash(string accessToken, int signingAlgorithmBits, Claims claims)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("validate authorization code hash");

      var atHash = claims.FindFirst(JwtClaimTypes.AccessTokenHash)?.Value ?? "";
      if (atHash.IsMissing())
      {
        return true;
      }

      var hashAlgorithm = GetHashAlgorithm(signingAlgorithmBits);
      if (hashAlgorithm == null)
      {
        s_logger.LogError("No appropriate hashing algorithm found.");
      }

      var codeHash = hashAlgorithm.HashData(
          CryptographicBuffer.CreateFromByteArray(
              Encoding.UTF8.GetBytes(accessToken)));

      byte[] atHashArray;
      CryptographicBuffer.CopyToByteArray(codeHash, out atHashArray);

      byte[] leftPart = new byte[signingAlgorithmBits / 16];
      Array.Copy(atHashArray, leftPart, signingAlgorithmBits / 16);

      var leftPartB64 = Base64Url.Encode(leftPart);

      var match = leftPartB64.Equals(atHash);

      if (!match)
      {
        s_logger.LogError($"access token hash ({leftPartB64}) does not match at_hash from token ({atHash})");
      }

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("success");
      return match;
    }

    private async Task<TokenResponse> RedeemCodeAsync(string code, AuthorizeState state)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Redeeming authorization code");

      var client = await GetTokenClientAsync();

      var tokenResult = await client.RequestAuthorizationCodeAsync(
          code,
          state.RedirectUri,
          codeVerifier: state.CodeVerifier);

      return tokenResult;
    }


    private IHashAlgorithmProvider GetHashAlgorithm(int signingAlgorithmBits)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"determining hash algorithm for {signingAlgorithmBits} bits");

      switch (signingAlgorithmBits)
      {
        case 256:
          if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("SHA256");
          return HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
        case 384:
          if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("SHA384");
          return HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha384);
        case 512:
          if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("SHA512");
          return HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha512);
        default:
          return null;
      }
    }

    private async Task<TokenClient> GetTokenClientAsync()
    {
      if (_tokenClient == null)
      {
        _tokenClient = await TokenClientFactory.CreateAsync(_options);
      }

      return _tokenClient;
    }
  }
}