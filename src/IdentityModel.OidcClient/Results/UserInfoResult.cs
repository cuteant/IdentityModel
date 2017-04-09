using System.Collections.Generic;
#if NET40
using CuteAnt.Security.Claims;
#else
using System.Security.Claims;
#endif

namespace IdentityModel.OidcClient.Results
{
    public class UserInfoResult : Result
    {
        public IEnumerable<Claim> Claims { get; set; }
    }
}