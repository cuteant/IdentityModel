/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using CuteAnt.Owin.Security.Jwt;
using CuteAnt.Owin.Security.OAuth;
using System;
using System.Security.Cryptography.X509Certificates;

namespace CuteAnt.Owin
{
    public static class IdentityModelJwtBearerAuthenticationExtensions
    {
        public static IAppBuilder UseJsonWebToken(this IAppBuilder app, string issuer, string audience, string signingKey, OAuthBearerAuthenticationProvider location = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            var options = new JwtBearerAuthenticationOptions
            {
                AllowedAudiences = new[] { audience },
                IssuerSecurityTokenProviders = new[] 
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(
                            issuer,
                            signingKey)
                    }
            };

            if (location != null)
            {
                options.Provider = location;
            }

            app.UseJwtBearerAuthentication(options);
            
            return app;
        }

        public static IAppBuilder UseJsonWebToken(this IAppBuilder app, string issuer, string audience, X509Certificate2 signingKey, OAuthBearerAuthenticationProvider location = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            var options = new JwtBearerAuthenticationOptions
            {
                AllowedAudiences = new[] { audience },
                IssuerSecurityTokenProviders = new[] 
                    {
                        new X509CertificateSecurityTokenProvider(
                            issuer,
                            signingKey)
                    }
            };

            if (location != null)
            {
                options.Provider = location;
            }

            app.UseJwtBearerAuthentication(options);

            return app;
        }
    }
}