using System.Collections.Generic;

namespace IdentityModel.Owin.ScopeValidation
{
    public class ScopeValidationOptions
    {
        public string ScopeClaimType { get; set; }
        public bool AllowAnonymousAccess { get; set; }

        public IEnumerable<string> Scopes { get; set; }

        public ScopeValidationOptions()
        {
            ScopeClaimType = "scope";
            AllowAnonymousAccess = false;
            Scopes = new List<string>();
        }
    }
}