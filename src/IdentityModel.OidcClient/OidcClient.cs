// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.OidcClient.Infrastructure;
#if NET40
using CuteAnt.Extensions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace IdentityModel.OidcClient
{
  /// <summary>
  /// OpenID Connect client
  /// </summary>
  public class OidcClient
  {
    private static ILogger s_logger = TraceLogger.GetLogger<OidcClient>();

    private readonly AuthorizeClient _authorizeClient;
    private readonly OidcClientOptions _options;
    private readonly ResponseValidator _validator;

    /// <summary>
    /// Gets the options.
    /// </summary>
    /// <value>
    /// The options.
    /// </value>
    public OidcClientOptions Options
    {
      get { return _options; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OidcClient"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public OidcClient(OidcClientOptions options)
    {
      _authorizeClient = new AuthorizeClient(options);
      _validator = new ResponseValidator(options);

      _options = options;
    }

    /// <summary>
    /// Starts an authentication request.
    /// </summary>
    /// <param name="trySilent">if set to <c>true</c> a silent login attempt is made.</param>
    /// <param name="extraParameters">extra parameters to send to the authorize endpoint.</param>
    /// <returns></returns>
    public async Task<LoginResult> LoginAsync(bool trySilent = false, object extraParameters = null)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("LoginAsync");

      var authorizeResult = await _authorizeClient.AuthorizeAsync(trySilent, extraParameters);

      if (!authorizeResult.Success)
      {
        return new LoginResult(authorizeResult.Error);
      }

      return await ValidateResponseAsync(authorizeResult.Data, authorizeResult.State);
    }

    /// <summary>
    /// Prepares an authentication request.
    /// </summary>
    /// <param name="extraParameters">extra parameters to send to the authorize endpoint.</param>
    /// <returns>An authorize state object that can be later used to validate the response</returns>
    public async Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("PrepareLoginAsync");

      return await _authorizeClient.PrepareAuthorizeAsync(extraParameters);
    }

    /// <summary>
    /// Starts and end session request.
    /// </summary>
    /// <param name="identityToken">An identity token to send as a hint.</param>
    /// <param name="trySilent">if set to <c>true</c> a silent end session attempt is made.</param>
    /// <returns></returns>
    public Task LogoutAsync(string identityToken = null, bool trySilent = true)
    {
      return _authorizeClient.EndSessionAsync(identityToken, trySilent);
    }

    /// <summary>
    /// Validates the response.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <param name="state">The state.</param>
    /// <returns>Result of the login response validation</returns>
    /// <exception cref="System.InvalidOperationException">Invalid authentication style</exception>
    public async Task<LoginResult> ValidateResponseAsync(string data, AuthorizeState state)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Validate authorize response");

      var response = new AuthorizeResponse(data);

      if (response.IsError)
      {
        s_logger.LogError(response.Error);

        return new LoginResult(response.Error);
      }

      if (string.IsNullOrEmpty(response.Code))
      {
        var error = "Missing authorization code";
        s_logger.LogError(error);

        return new LoginResult(error);
      }

      if (string.IsNullOrEmpty(response.State))
      {
        var error = "Missing state";
        s_logger.LogError(error);

        return new LoginResult(error);
      }

      if (!string.Equals(state.State, response.State, StringComparison.Ordinal))
      {
        var error = "Invalid state";
        s_logger.LogError(error);

        return new LoginResult(error);
      }

      ResponseValidationResult validationResult = null;
      if (_options.Style == OidcClientOptions.AuthenticationStyle.AuthorizationCode)
      {
        validationResult = await _validator.ValidateCodeFlowResponseAsync(response, state);
      }
      else if (_options.Style == OidcClientOptions.AuthenticationStyle.Hybrid)
      {
        validationResult = await _validator.ValidateHybridFlowResponseAsync(response, state);
      }
      else
      {
        throw new InvalidOperationException("Invalid authentication style");
      }

      if (!validationResult.Success)
      {
        s_logger.LogError("Error validating response: " + validationResult.Error);

        return new LoginResult
        {
          Error = validationResult.Error
        };
      }

      return await ProcessClaimsAsync(validationResult);
    }

    /// <summary>
    /// Gets the user claims from the userinfo endpoint.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    /// <returns>User claims</returns>
    public async Task<UserInfoResult> GetUserInfoAsync(string accessToken)
    {
      var providerInfo = await _options.GetProviderInformationAsync();

      if (accessToken.IsMissing()) throw new ArgumentNullException(nameof(accessToken));
      if (providerInfo.UserInfoEndpoint.IsMissing()) throw new InvalidOperationException("No userinfo endpoint specified");

      var handler = _options.BackchannelHandler ?? new HttpClientHandler();

      var userInfoClient = new UserInfoClient(new Uri(providerInfo.UserInfoEndpoint), accessToken, handler);
      userInfoClient.Timeout = _options.BackchannelTimeout;

      var userInfoResponse = await userInfoClient.GetAsync();
      if (userInfoResponse.IsError)
      {
        return new UserInfoResult
        {
          Error = userInfoResponse.ErrorMessage
        };
      }

      return new UserInfoResult
      {
        Claims = userInfoResponse.Claims.Select(c => new Claim(c.Item1, c.Item2)).ToClaims()
      };
    }

    /// <summary>
    /// Starts a refresh token request.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>A refresh token result</returns>
    public async Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
    {
      var client = await TokenClientFactory.CreateAsync(_options);
      var response = await client.RequestRefreshTokenAsync(refreshToken);

      if (response.IsError)
      {
        return new RefreshTokenResult { Error = response.Error };
      }

      // validate token response
      var validationResult = await _validator.ValidateTokenResponse(response, requireIdentityToken: false);
      if (!validationResult.Success)
      {
        return new RefreshTokenResult { Error = validationResult.Error };
      }

      return new RefreshTokenResult
      {
        AccessToken = response.AccessToken,
        RefreshToken = response.RefreshToken,
        ExpiresIn = (int)response.ExpiresIn
      };
    }

    private async Task<LoginResult> ProcessClaimsAsync(ResponseValidationResult result)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("Processing claims");

      // get profile if enabled
      if (_options.LoadProfile)
      {
        if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("load profile");

        var userInfoResult = await GetUserInfoAsync(result.TokenResponse.AccessToken);

        if (!userInfoResult.Success)
        {
          return new LoginResult(userInfoResult.Error);
        }

        if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("profile claims:");
        s_logger.LogClaims(userInfoResult.Claims);

        var primaryClaimTypes = result.Claims.Select(c => c.Type).Distinct();
        foreach (var claim in userInfoResult.Claims.Where(c => !primaryClaimTypes.Contains(c.Type)))
        {
          result.Claims.Add(claim);
        }
      }
      else
      {
        if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("don't load profile");
      }

      // success
      var loginResult = new LoginResult
      {
        Claims = FilterClaims(result.Claims),
        AccessToken = result.TokenResponse.AccessToken,
        RefreshToken = result.TokenResponse.RefreshToken,
        AccessTokenExpiration = DateTime.Now.AddSeconds(result.TokenResponse.ExpiresIn),
        IdentityToken = result.TokenResponse.IdentityToken,
        AuthenticationTime = DateTime.Now
      };

      if (!string.IsNullOrWhiteSpace(result.TokenResponse.RefreshToken))
      {
        var providerInfo = await _options.GetProviderInformationAsync();

        loginResult.Handler = new RefeshTokenHandler(
            await TokenClientFactory.CreateAsync(_options),
            result.TokenResponse.RefreshToken,
            result.TokenResponse.AccessToken);
      }

      return loginResult;
    }

    private Claims FilterClaims(Claims claims)
    {
      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("filtering claims");

      if (_options.FilterClaims)
      {
        claims = claims.Where(c => !_options.FilteredClaims.Contains(c.Type)).ToClaims();
      }

      if (s_logger.IsDebugLevelEnabled()) s_logger.LogDebug("filtered claims:");
      s_logger.LogClaims(claims);

      return claims;
    }
  }
}