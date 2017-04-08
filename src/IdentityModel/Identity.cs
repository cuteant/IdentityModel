/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace IdentityModel
{
  /// <summary>Helpers to create ClaimsIdentity</summary>
  public static class Identity
  {
    /// <summary>Creates an anonymous claims identity.</summary>
    /// <value>The anonymous.</value>
    public static ClaimsIdentity Anonymous
    {
      get
      {
        var claims = new List<Claim>
        {
          new Claim(ClaimTypes.Name, "")
        };

        return new ClaimsIdentity(claims);
      }
    }

    /// <summary>Creates a ClaimsIdentity using the specified authentication type and claims.</summary>
    /// <param name="authenticationType">Type of the authentication.</param>
    /// <param name="claims">The claims.</param>
    /// <returns></returns>
    public static ClaimsIdentity Create(string authenticationType, params Claim[] claims)
    {
      return new ClaimsIdentity(claims, authenticationType);
    }

    /// <summary>Creates a ClaimsIdentity based on information found in an X509 certificate.</summary>
    /// <param name="certificate">The certificate.</param>
    /// <param name="authenticationType">Type of the authentication.</param>
    /// <param name="includeAllClaims">if set to <c>true</c> [include all claims].</param>
    /// <returns></returns>
    public static ClaimsIdentity CreateFromCertificate(X509Certificate2 certificate, string authenticationType = "X.509", bool includeAllClaims = false)
    {
      var claims = new List<Claim>();
      var issuer = certificate.Issuer;

      claims.Add(new Claim("issuer", issuer));

      var thumbprint = certificate.Thumbprint;
      claims.Add(new Claim(ClaimTypes.Thumbprint, thumbprint, ClaimValueTypes.Base64Binary, issuer));

      var name = certificate.SubjectName.Name;
      if (!string.IsNullOrEmpty(name))
      {
        claims.Add(new Claim(ClaimTypes.X500DistinguishedName, name, ClaimValueTypes.String, issuer));
      }

      if (includeAllClaims)
      {
        name = certificate.SerialNumber;
        if (!string.IsNullOrEmpty(name))
        {
          claims.Add(new Claim(ClaimTypes.SerialNumber, name, "http://www.w3.org/2001/XMLSchema#string", issuer));
        }

        name = certificate.GetNameInfo(X509NameType.DnsName, false);
        if (!string.IsNullOrEmpty(name))
        {
          claims.Add(new Claim(ClaimTypes.Dns, name, ClaimValueTypes.String, issuer));
        }

        name = certificate.GetNameInfo(X509NameType.SimpleName, false);
        if (!string.IsNullOrEmpty(name))
        {
          claims.Add(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, issuer));
        }

        name = certificate.GetNameInfo(X509NameType.EmailName, false);
        if (!string.IsNullOrEmpty(name))
        {
          claims.Add(new Claim(ClaimTypes.Email, name, ClaimValueTypes.String, issuer));
        }

        name = certificate.GetNameInfo(X509NameType.UpnName, false);
        if (!string.IsNullOrEmpty(name))
        {
          claims.Add(new Claim(ClaimTypes.Upn, name, ClaimValueTypes.String, issuer));
        }

        name = certificate.GetNameInfo(X509NameType.UrlName, false);
        if (!string.IsNullOrEmpty(name))
        {
          claims.Add(new Claim(ClaimTypes.Uri, name, ClaimValueTypes.String, issuer));
        }

#if DESKTOPCLR
        if (certificate.PublicKey.Key is RSA key)
        {
          claims.Add(new Claim(ClaimTypes.Rsa, key.ToXmlString(false), ClaimValueTypes.RsaKeyValue, issuer));
        }

        if (certificate.PublicKey.Key is DSA dsa)
        {
          claims.Add(new Claim(ClaimTypes.Dsa, dsa.ToXmlString(false), ClaimValueTypes.DsaKeyValue, issuer));
        }

        var expiration = certificate.GetExpirationDateString();
        if (!string.IsNullOrEmpty(expiration))
        {
          claims.Add(new Claim(ClaimTypes.Expiration, expiration, ClaimValueTypes.DateTime, issuer));
        }
#endif
      }

      return new ClaimsIdentity(claims, authenticationType);
    }
  }
}