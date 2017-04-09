/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using CuteAnt.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin
{
    public class QueryStringOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        readonly string _name;

        public QueryStringOAuthBearerProvider(string name)
        {
            _name = name;
        }

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            context.Token = context.Request.Query.Get(_name);

            return Task.FromResult<object>(null);
        }
    }
}