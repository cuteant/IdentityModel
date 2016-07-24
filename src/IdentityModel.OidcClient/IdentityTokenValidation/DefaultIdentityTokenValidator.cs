// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using System.Linq;
using JosePCL.Keys.Rsa;
using Newtonsoft.Json.Linq;
#if NET40
using CuteAnt.Extensions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace IdentityModel.OidcClient.IdentityTokenValidation
{
  public class DefaultIdentityTokenValidator : IIdentityTokenValidator
  {
    private static readonly ILogger s_logger = TraceLogger.GetLogger<DefaultIdentityTokenValidator>();

    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);

    public Task<IdentityTokenValidationResult> ValidateAsync(string identityToken, string clientId, ProviderInformation providerInformation)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("starting identity token validation");
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"identity token: {identityToken}");

      var fail = new IdentityTokenValidationResult
      {
        Success = false
      };

      var e = Base64Url.Decode(providerInformation.KeySet.Keys.First().E);
      var n = Base64Url.Decode(providerInformation.KeySet.Keys.First().N);
      var pubKey = PublicKey.New(e, n);

      var json = JosePCL.Jwt.Decode(identityToken, pubKey);
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("decoded JWT: " + json);

      var payload = JObject.Parse(json);

      var issuer = payload["iss"].ToString();
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"issuer: {issuer}");

      var audience = payload["aud"].ToString();
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug($"audience: {audience}");

      if (issuer != providerInformation.IssuerName)
      {
        fail.Error = "Invalid issuer name";
        s_logger.LogError(fail.Error);

#if NET40
        return TaskEx.FromResult(fail);
#else
        return Task.FromResult(fail);
#endif
      }

      if (audience != clientId)
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
      var exp = payload.Value<long>("exp");
      var nbf = payload.Value<long?>("nbf");

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
        Success = true,
        Claims = payload.ToClaims(),
        SignatureAlgorithm = "RS256"
      });

    }
  }
}