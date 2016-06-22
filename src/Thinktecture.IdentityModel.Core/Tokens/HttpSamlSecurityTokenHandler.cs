/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.IdentityModel.Tokens;
using System.IO;
using System.Xml;

namespace Thinktecture.IdentityModel.Tokens
{
    public class HttpSamlSecurityTokenHandler : SamlSecurityTokenHandler
    {
        public HttpSamlSecurityTokenHandler()
            : base()
        { }

        public HttpSamlSecurityTokenHandler(SamlSecurityTokenRequirement requirement)
            : base(requirement)
        { }

        public override SecurityToken ReadToken(string tokenString)
        {
            // unbase64 header if necessary
            if (HeaderEncoding.IsBase64Encoded(tokenString))
            {
                tokenString = HeaderEncoding.DecodeBase64(tokenString);
            }

            // check containing collection (mainly useful for chained EncryptedSecurityTokenHandler)
            if (ContainingCollection != null)
            {
                return ContainingCollection.ReadToken(new XmlTextReader(new StringReader(tokenString)));
            }
            else
            {
                return ReadToken(new XmlTextReader(new StringReader(tokenString)));
            }
        }
    }
}