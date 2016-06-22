/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using CuteAnt.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace IdentityModel.Owin
{
    public class HeaderOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        readonly string _name;

        public HeaderOAuthBearerProvider(string name)
        {
            _name = name;
        }

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            context.Token = context.Request.Headers.Get(_name);

            return Task.FromResult<object>(null);
        }
    }
}