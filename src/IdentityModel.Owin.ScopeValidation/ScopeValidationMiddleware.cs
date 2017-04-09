using CuteAnt.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin.ScopeValidation
{
    public class ScopeValidationMiddleware
    {
        readonly Func<IDictionary<string, object>, Task> _next;
        private ScopeValidationOptions _options;

        public ScopeValidationMiddleware(Func<IDictionary<string, object>, Task> next, ScopeValidationOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);

            if (AccessAllowed(context))
            {
                await _next(env);
            }
            else
            {
                context.Response.StatusCode = 403;
                context.Response.Headers.Add("WWW-Authenticate", new[] { "Bearer error=\"insufficient_scope\"" });
                
                return;
            }
        }

        private bool AccessAllowed(OwinContext context)
        {
            var principal = context.Authentication.User;
            if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return _options.AllowAnonymousAccess;
            }

            if (_options.Scopes == null || _options.Scopes.Count() == 0)
            {
                return true;
            }

            var scopeClaims = principal.FindAll(_options.ScopeClaimType);

            if (scopeClaims == null || scopeClaims.Count() == 0)
            {
                return false;
            }

            foreach (var scope in scopeClaims)
            {
                if (_options.Scopes.Contains(scope.Value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}