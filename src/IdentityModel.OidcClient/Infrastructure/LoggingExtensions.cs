using System.Diagnostics;
using IdentityModel.OidcClient;

#if NET40
namespace CuteAnt.Extensions.Logging
#else
namespace Microsoft.Extensions.Logging
#endif
{
  internal static class LoggingExtensions
  {
    [DebuggerStepThrough]
    public static void LogClaims(this ILogger logger, Claims claims)
    {
      if (logger.IsDebugLevelEnabled())
      {
        foreach (var claim in claims)
        {
          logger.LogDebug($"Claim: {claim.Type}: {claim.Value}");
        }
      }
    }
  }
}