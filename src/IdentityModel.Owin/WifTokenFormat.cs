/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using CuteAnt.Owin.Security;
using System;
using System.IdentityModel.Tokens;
using System.Linq;

namespace Thinktecture.IdentityModel.Owin
{
    public class WifTokenFormat : ISecureDataFormat<AuthenticationTicket>
    {
        SecurityTokenHandler _handler;

        public WifTokenFormat(SecurityTokenHandler handler)
        {
            _handler = handler;
        }


        public string Protect(AuthenticationTicket data)
        {
            throw new NotSupportedException();
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            if (string.IsNullOrWhiteSpace(protectedText))
            {
                throw new ArgumentNullException("protectedText");
            }

            var token = _handler.ReadToken(protectedText);
            var identity = _handler.ValidateToken(token);

            return new AuthenticationTicket(identity.First(), new AuthenticationProperties());
        }
    }
}