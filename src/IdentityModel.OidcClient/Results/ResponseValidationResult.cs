using IdentityModel.Client;
#if NET40
using CuteAnt.Security.Claims;
#else
using System.Security.Claims;
#endif

namespace IdentityModel.OidcClient
{
    public class ResponseValidationResult : Result
    {
        public ResponseValidationResult()
        {

        }

        public ResponseValidationResult(string error)
        {
            Error = error;
        }

        public AuthorizeResponse AuthorizeResponse { get; set; }
        public TokenResponse TokenResponse { get; set; }
        public ClaimsPrincipal User { get; set; }
    }
}