/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace Thinktecture.IdentityModel.Metadata
{
    public class FederationMetadata
    {
        public static IEnumerable<X509Certificate> GetSigningCertificates(Uri url, X509CertificateValidationMode mode = X509CertificateValidationMode.None)
        {
            var certs = new List<X509Certificate2>();

            using (var stream = GetMetadataStream(url))
            {
                var serializer = new MetadataSerializer();
                serializer.CertificateValidationMode = mode;

                var md = serializer.ReadMetadata(stream);
                var ed = md as EntityDescriptor;
                var stsd = (SecurityTokenServiceDescriptor)ed.RoleDescriptors.FirstOrDefault(x => x is SecurityTokenServiceDescriptor);

                foreach (var key in stsd.Keys)
                {
                    var clause = key.KeyInfo.FirstOrDefault() as X509RawDataKeyIdentifierClause;
                    if (clause != null)
                    {
                        var cert = new X509Certificate2(clause.GetX509RawData());
                        certs.Add(cert);
                    }
                }
            }

            return certs;
        }

        private static Stream GetMetadataStream(Uri url)
        {
            var client = new HttpClient { BaseAddress = url };
            var stream = client.GetStreamAsync("").Result;
            return stream;
        }
    }
}
