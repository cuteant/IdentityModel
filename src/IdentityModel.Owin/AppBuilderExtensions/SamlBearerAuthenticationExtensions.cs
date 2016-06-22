/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using CuteAnt.Owin.Security.OAuth;
using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using IdentityModel.Owin;
using Thinktecture.IdentityModel.Tokens;

namespace CuteAnt.Owin
{
    public static class SamlBearerAuthenticationExtensions
    {
        public static IAppBuilder UseSaml2BearerAuthentication(this IAppBuilder app, Uri audience, string issuerThumbprint, string issuerName = null, X509CertificateValidator validator = null)
        {
            var handler = new HttpSaml2SecurityTokenHandler();
            ConfigureHandler(handler, audience, issuerThumbprint, issuerName, validator);

            return app.UseTokenHandlerAuthentication(handler);
        }

        public static IAppBuilder UseSaml11BearerAuthentication(this IAppBuilder app, Uri audience, string issuerThumbprint, string issuerName = null, X509CertificateValidator validator = null)
        {
            var handler = new HttpSamlSecurityTokenHandler();
            ConfigureHandler(handler, audience, issuerThumbprint, issuerName, validator);

            return app.UseTokenHandlerAuthentication(handler);
        }

        public static IAppBuilder UseTokenHandlerAuthentication(this IAppBuilder app, SecurityTokenHandler handler)
        {
            var options = new OAuthBearerAuthenticationOptions
            {
                AccessTokenFormat = new WifTokenFormat(handler)
            };

            app.UseOAuthBearerAuthentication(options);

            return app;
        }

        private static void ConfigureHandler(SecurityTokenHandler handler, Uri audience, string issuerThumbprint, string issuerName = null, X509CertificateValidator validator = null)
        {
            var handlerConfiguration = new SecurityTokenHandlerConfiguration();
            handlerConfiguration.AudienceRestriction.AllowedAudienceUris.Add(audience);

            var registry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer(issuerThumbprint, issuerName ?? issuerThumbprint);

            if (validator != null)
            {
                handlerConfiguration.CertificateValidator = validator;
            }
            else
            {
                handlerConfiguration.CertificateValidator = X509CertificateValidator.None;
            }

            handlerConfiguration.IssuerNameRegistry = registry;
            handler.Configuration = handlerConfiguration;
        }

    }
}