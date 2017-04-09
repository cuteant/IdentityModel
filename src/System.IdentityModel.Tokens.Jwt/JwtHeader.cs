//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;

namespace System.IdentityModel.Tokens.Jwt
{
    /// <summary>
    /// Initializes a new instance of <see cref="JwtHeader"/> which contains JSON objects representing the cryptographic operations applied to the JWT and optionally any additional properties of the JWT. 
    /// The member names within the JWT Header are referred to as Header Parameter Names.
    /// <para>These names MUST be unique and the values must be <see cref="string"/>(s). The corresponding values are referred to as Header Parameter Values.</para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable"), System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Serialize not really supported.")]
    public class JwtHeader : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtHeader"/> class. Default string comparer <see cref="StringComparer.Ordinal"/>.
        /// </summary>
        public JwtHeader()
            : this((SigningCredentials)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JwtHeader"/>.
        /// With the Header Parameters:
        /// <para>{ { typ, JWT }, { alg, SigningCredentials.Algorithm } }</para>
        /// </summary>
        /// <param name="signingCredentials"><see cref="SigningCredentials"/> used creating a JWS Compact JSON.</param>
        public JwtHeader(SigningCredentials signingCredentials)
            : this(signingCredentials, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JwtHeader"/>.
        /// With the Header Parameters:
        /// <para>{ { typ, JWT }, { alg, EncryptingCredentials.Alg }, { enc, EncryptingCredentials.Enc } }</para>
        /// </summary>
        /// <param name="encryptingCredentials"><see cref="EncryptingCredentials"/> used creating a JWE Compact JSON.</param>
        /// <exception cref="ArgumentNullException">If 'encryptingCredentials' is null.</exception>
        public JwtHeader(EncryptingCredentials encryptingCredentials)
            : this(encryptingCredentials, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JwtHeader"/>.
        /// With the Header Parameters:
        /// <para>{ { typ, JWT }, { alg, SigningCredentials.Algorithm } }</para>
        /// </summary>
        /// <param name="signingCredentials"><see cref="SigningCredentials"/> used when creating a JWS Compact JSON.</param>
        /// <param name="outboundAlgorithmMap">provides a mapping for the 'alg' value so that values are within the JWT namespace.</param>
        public JwtHeader(SigningCredentials signingCredentials, IDictionary<string,string> outboundAlgorithmMap)
            : base(StringComparer.Ordinal)
        {
            if (signingCredentials == null)
            {
                this[JwtHeaderParameterNames.Alg] = SecurityAlgorithms.None;
            }
            else
            {
                string outboundAlg;
                if (outboundAlgorithmMap != null && outboundAlgorithmMap.TryGetValue(signingCredentials.Algorithm, out outboundAlg))
                    Alg = outboundAlg;
                else
                    Alg = signingCredentials.Algorithm;

                if (!string.IsNullOrEmpty(signingCredentials.Key.KeyId))
                    Kid = signingCredentials.Key.KeyId;
            }

            Typ = JwtConstants.HeaderType;
            SigningCredentials = signingCredentials;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JwtHeader"/>.
        /// With the Header Parameters:
        /// <para>{ { typ, JWT }, { alg, SigningCredentials.Algorithm } }</para>
        /// </summary>
        /// <param name="encryptingCredentials"><see cref="EncryptingCredentials"/> used when creating a JWS Compact JSON.</param>
        /// <param name="outboundAlgorithmMap">provides a mapping for the 'alg' value so that values are within the JWT namespace.</param>
        /// <exception cref="ArgumentNullException">If 'signingCredentials' is null.</exception>
        public JwtHeader(EncryptingCredentials encryptingCredentials, IDictionary<string, string> outboundAlgorithmMap)
            : base(StringComparer.Ordinal)
        {
            if (encryptingCredentials == null)
                throw LogHelper.LogArgumentNullException(nameof(encryptingCredentials));

            string outboundAlg;
            if (outboundAlgorithmMap != null && outboundAlgorithmMap.TryGetValue(encryptingCredentials.Alg, out outboundAlg))
                Alg = outboundAlg;
            else
                Alg = encryptingCredentials.Alg;

            if (outboundAlgorithmMap != null && outboundAlgorithmMap.TryGetValue(encryptingCredentials.Enc, out outboundAlg))
                Enc = outboundAlg;
            else
                Enc = encryptingCredentials.Enc;

            if (!string.IsNullOrEmpty(encryptingCredentials.Key.KeyId))
                Kid = encryptingCredentials.Key.KeyId;

            Typ = JwtConstants.HeaderType;

            EncryptingCredentials = encryptingCredentials;
        }

        /// <summary>
        /// Gets the signature algorithm that was used to create the signature.
        /// </summary>
        /// <remarks>If the signature algorithm is not found, null is returned.</remarks>
        public string Alg
        {
            get
            {
                return GetStandardClaim(JwtHeaderParameterNames.Alg);
            }

            private set
            {
                this[JwtHeaderParameterNames.Alg] = value;
            }
        }

        /// <summary>
        /// Gets the content mime type (Cty) of the token.
        /// </summary>
        /// <remarks>If the content mime type is not found, null is returned.</remarks>
        public string Cty
        {
            get
            {
                return GetStandardClaim(JwtHeaderParameterNames.Cty);
            }

            private set
            {
                this[JwtHeaderParameterNames.Cty] = value;
            }
        }

        /// <summary>
        /// Gets the encryption algorithm (Enc) of the token.
        /// </summary>
        /// <remarks>If the content mime type is not found, null is returned.</remarks>
        public string Enc
        {
            get
            {
                return GetStandardClaim(JwtHeaderParameterNames.Enc);
            }

            private set
            {
                this[JwtHeaderParameterNames.Enc] = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="EncryptingCredentials"/> passed in the constructor.
        /// </summary>
        /// <remarks>This value may be null.</remarks>
        public EncryptingCredentials EncryptingCredentials { get; private set; }

        /// <summary>
        /// Gets the iv of symmetric key wrap.
        /// </summary>
        public string IV
        {
            get
            {
                return GetStandardClaim(JwtHeaderParameterNames.IV);
            }
        }

        /// <summary>
        /// Gets the key identifier for the security key used to sign the token
        /// </summary>
        public string Kid
        {
            get
            {
                return GetStandardClaim(JwtHeaderParameterNames.Kid);
            }

            private set
            {
                this[JwtHeaderParameterNames.Kid] = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="SigningCredentials"/> passed in the constructor.
        /// </summary>
        /// <remarks>This value may be null.</remarks>
        public SigningCredentials SigningCredentials
        {
            get; private set;
        }

        /// <summary>
        /// Gets the mime type (Typ) of the token.
        /// </summary>
        /// <remarks>If the mime type is not found, null is returned.</remarks>
        public string Typ
        {
            get
            {
                return GetStandardClaim(JwtHeaderParameterNames.Typ);
            }

            private set
            {
                this[JwtHeaderParameterNames.Typ] = value;
            }
        }

        /// <summary>
        /// Gets the thumbprint of the certificate used to sign the token
        /// </summary>
        public string X5t
        {
            get
            {
                return GetStandardClaim(JwtHeaderParameterNames.X5t);
            }
        }

        /// <summary>
        /// Deserializes Base64UrlEncoded JSON into a <see cref="JwtHeader"/> instance.
        /// </summary>
        /// <param name="base64UrlEncodedJsonString">Base64url encoded JSON to deserialize.</param>
        /// <returns>An instance of <see cref="JwtHeader"/>.</returns>
        /// <remarks>Use <see cref="JsonExtensions.Deserializer"/> to customize JSON serialization.</remarks>
        public static JwtHeader Base64UrlDeserialize(string base64UrlEncodedJsonString)
        {
            return JsonExtensions.DeserializeJwtHeader(Base64UrlEncoder.Decode(base64UrlEncodedJsonString));
        }

        /// <summary>
        /// Encodes this instance as Base64UrlEncoded JSON.
        /// </summary>
        /// <returns>Base64UrlEncoded JSON.</returns>
        /// <remarks>Use <see cref="JsonExtensions.Serializer"/> to customize JSON serialization.</remarks>
        public virtual string Base64UrlEncode()
        {
            return Base64UrlEncoder.Encode(SerializeToJson());
        }

        /// <summary>
        /// Deserialzes JSON into a <see cref="JwtHeader"/> instance.
        /// </summary>
        /// <param name="jsonString"> The JSON to deserialize.</param>
        /// <returns>An instance of <see cref="JwtHeader"/>.</returns>
        /// <remarks>Use <see cref="JsonExtensions.Deserializer"/> to customize JSON serialization.</remarks>
        public static JwtHeader Deserialize(string jsonString)
        {
            return JsonExtensions.DeserializeJwtHeader(jsonString);
        }
        /// <summary>
        /// Gets a standard claim from the header.
        /// A standard cliam is either a string or a value of another type serialized in JSON format.
        /// </summary>
        /// <param name="claimType">The key of the claim.</param>
        /// <returns>The standard claim string; or null if not found.</returns>
        internal string GetStandardClaim(string claimType)
        {
            object value = null;
            if (TryGetValue(claimType, out value))
            {
                string str = value as string;
                if (str != null)
                {
                    return str;
                }

                return JsonExtensions.SerializeToJson(value);
            }

            return null;
        }

        /// <summary>
        /// Serializes this instance to JSON.
        /// </summary>
        /// <returns>This instance as JSON.</returns>
        /// <remarks>Use <see cref="JsonExtensions.Serializer"/> to customize JSON serialization.</remarks>
        public virtual string SerializeToJson()
        {
            return JsonExtensions.SerializeToJson(this as IDictionary<string, object>);
        }
    }
}
