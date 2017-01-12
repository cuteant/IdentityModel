using System.Collections.Generic;
using System.Security.Claims;

namespace CuteAnt.IdentityModel.OidcClient.Results
{
    public class UserInfoResult : Result
    {
        public IEnumerable<Claim> Claims { get; set; }
    }
}