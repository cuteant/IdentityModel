using CuteAnt.IdentityModel.Client;
using System.Security.Claims;

namespace CuteAnt.IdentityModel.OidcClient
{
    public class ResponseValidationResult : Result
    {
        public AuthorizeResponse AuthorizeResponse { get; set; }
        public TokenResponse TokenResponse { get; set; }
        public ClaimsPrincipal User { get; set; }
    }
}