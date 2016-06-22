/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * Contribution by Pedro Felix
 * see LICENSE
 */

using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Thinktecture.IdentityModel.Http
{
    public class CertPinningWebRequestHandler : WebRequestHandler
    {
        ThumbprintSet _thumbprints;

        public CertPinningWebRequestHandler(ThumbprintSet thumbprints)
        {
            _thumbprints = thumbprints;
            this.ServerCertificateValidationCallback = CertificateValidationCallback;
        }

        bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var caCerts = chain.ChainElements
                               .Cast<X509ChainElement>().Skip(1)
                               .Select(elem => elem.Certificate);

            return sslPolicyErrors == SslPolicyErrors.None &&
                   caCerts.Any(cert => _thumbprints.Contains(cert.GetCertHashString()));
        }
    }
}
