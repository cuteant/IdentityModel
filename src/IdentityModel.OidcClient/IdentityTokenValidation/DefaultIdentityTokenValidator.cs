// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using System.Linq;
using JosePCL.Keys.Rsa;
using Newtonsoft.Json.Linq;
using IdentityModel.Jwt;
using JosePCL.Serialization;
using PCLCrypto;
using System.Collections.Generic;
#if NET40
using CuteAnt.Extensions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace IdentityModel.OidcClient.IdentityTokenValidation
{
  public class DefaultIdentityTokenValidator : IIdentityTokenValidator
  {
    private IEnumerable<string> _supportedAlgorithms = new List<string>
    {
      OidcConstants.Algorithms.Asymmetric.RS256,
      OidcConstants.Algorithms.Asymmetric.RS384,
      OidcConstants.Algorithms.Asymmetric.RS512
    };

    private static readonly ILogger s_logger = TraceLogger.GetLogger<DefaultIdentityTokenValidator>();

    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);

    public Task<IdentityTokenValidationResult> ValidateAsync(string identityToken, string clientId, ProviderInformation providerInformation)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("starting identity token validation");
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"identity token: {identityToken}");

      var fail = new IdentityTokenValidationResult();

      ValidatedToken token;
      try
      {
        token = ValidateSignature(identityToken, providerInformation.KeySet);
      }
      catch (Exception ex)
      {
        fail.Error = ex.ToString();
        s_logger.LogError(fail.Error);

#if NET40
        return TaskEx.FromResult(fail);
#else
        return Task.FromResult(fail);
#endif
      }

      if (!token.Success)
      {
        fail.Error = token.Error;
        s_logger.LogError(fail.Error);

#if NET40
        return TaskEx.FromResult(fail);
#else
        return Task.FromResult(fail);
#endif
      }

      var issuer = token.Payload["iss"].ToString();
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"issuer: {issuer}");

      var audience = token.Payload["aud"].ToString();
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"audience: {audience}");

      if (!string.Equals(issuer, providerInformation.IssuerName, StringComparison.Ordinal))
      {
        fail.Error = "Invalid issuer name";
        s_logger.LogError(fail.Error);

#if NET40
        return TaskEx.FromResult(fail);
#else
        return Task.FromResult(fail);
#endif
      }

      if (!string.Equals(audience, clientId, StringComparison.Ordinal))
      {
        fail.Error = "Invalid audience";
        s_logger.LogError(fail.Error);

#if NET40
        return TaskEx.FromResult(fail);
#else
        return Task.FromResult(fail);
#endif
      }

      var utcNow = DateTime.UtcNow;
      var exp = token.Payload.Value<long>("exp");
      var nbf = token.Payload.Value<long?>("nbf");

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"exp: {exp}");

      if (nbf != null)
      {
        if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"nbf: {nbf}");

        var notBefore = nbf.Value.ToDateTimeFromEpoch();
        if (notBefore > utcNow.Add(ClockSkew))
        {
          fail.Error = "Token not valid yet";
          s_logger.LogError(fail.Error);

#if NET40
          return TaskEx.FromResult(fail);
#else
          return Task.FromResult(fail);
#endif
        }
      }

      var expires = exp.ToDateTimeFromEpoch();
      if (expires < utcNow.Add(ClockSkew.Negate()))
      {
        fail.Error = "Token expired";
        s_logger.LogError(fail.Error);

#if NET40
        return TaskEx.FromResult(fail);
#else
        return Task.FromResult(fail);
#endif
      }

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogInformation("identity token validation success");

#if NET40
      return TaskEx.FromResult(new IdentityTokenValidationResult
#else
      return Task.FromResult(new IdentityTokenValidationResult
#endif
      {
        Claims = token.Payload.ToClaims(),
        SignatureAlgorithm = token.Algorithm
      });
    }

    ICryptographicKey LoadKey(JsonWebKeySet keySet, string kid)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"Searching keyset for id: {kid}");

      foreach (var webkey in keySet.Keys)
      {
        if (webkey.Kid == kid)
        {
          var e = Base64Url.Decode(webkey.E);
          var n = Base64Url.Decode(webkey.N);

          if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("found");
          return PublicKey.New(e, n);
        }
      }

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Key not found");
      return null;
    }

    ValidatedToken ValidateSignature(string token, JsonWebKeySet keySet)
    {
      var parts = Compact.Parse(token);
      var header = JObject.Parse(parts.First().Utf8);

      var kid = header["kid"].ToString();
      if (kid.IsMissing())
      {
        var error = "JWT has no kid";

        s_logger.LogError(error);
        return new ValidatedToken { Error = error };
      }

      var alg = header["alg"].ToString();

      if (!_supportedAlgorithms.Contains(alg))
      {
        var error = $"JWT uses an unsupported algorithm: {alg}";

        s_logger.LogError(error);
        return new ValidatedToken { Error = error };
      }

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Token signing algorithm: " + alg);

      var key = LoadKey(keySet, kid);
      if (key == null)
      {
        return new ValidatedToken
        {
          Error = "No key found that matches the kid of the token"
        };
      }

      var json = JosePCL.Jwt.Decode(token, key);
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("decoded JWT: " + json);

      var payload = JObject.Parse(json);

      return new ValidatedToken
      {
        KeyId = kid,
        Algorithm = alg,
        Payload = payload
      };
    }
  }
}