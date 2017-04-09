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

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// Log messages and codes
    /// </summary>
    internal static class LogMessages
    {
#pragma warning disable 1591
        // general
        internal const string IDX10000 = "IDX10000: The parameter '{0}' cannot be a 'null' or an empty object.";

        // properties, configuration 
        internal const string IDX10100 = "IDX10100: ClockSkew must be greater than TimeSpan.Zero. value: '{0}'";
        internal const string IDX10102 = "IDX10102: NameClaimType cannot be null or whitespace.";
        internal const string IDX10103 = "IDX10103: RoleClaimType cannot be null or whitespace.";

        // token validation
        internal const string IDX10204 = "IDX10204: Unable to validate issuer. validationParameters.ValidIssuer is null or whitespace AND validationParameters.ValidIssuers is null.";
        internal const string IDX10205 = "IDX10205: Issuer validation failed. Issuer: '{0}'. Did not match: validationParameters.ValidIssuer: '{1}' or validationParameters.ValidIssuers: '{2}'.";
        internal const string IDX10207 = "IDX10207: Unable to validate audience. The 'audiences' parameter is null.";
        internal const string IDX10208 = "IDX10208: Unable to validate audience. validationParameters.ValidAudience is null or whitespace and validationParameters.ValidAudiences is null.";
        internal const string IDX10211 = "IDX10211: Unable to validate issuer. The 'issuer' parameter is null or whitespace";
        internal const string IDX10214 = "IDX10214: Audience validation failed. Audiences: '{0}'. Did not match: validationParameters.ValidAudience: '{1}' or validationParameters.ValidAudiences: '{2}'.";
        internal const string IDX10222 = "IDX10222: Lifetime validation failed. The token is not yet valid.\nValidFrom: '{0}'\nCurrent time: '{1}'.";
        internal const string IDX10223 = "IDX10223: Lifetime validation failed. The token is expired.\nValidTo: '{0}'\nCurrent time: '{1}'.";
        internal const string IDX10224 = "IDX10224: Lifetime validation failed. The NotBefore: '{0}' is after Expires: '{1}'.";
        internal const string IDX10225 = "IDX10225: Lifetime validation failed. The token is missing an Expiration Time.\nTokentype: '{0}'.";
        internal const string IDX10227 = "IDX10227: TokenValidationParameters.TokenReplayCache is not null, indicating to check for token replay but the security token has no expiration time: token '{0}'.";
        internal const string IDX10228 = "IDX10228: The securityToken has previously been validated, securityToken: '{0}'.";
        internal const string IDX10229 = "IDX10229: TokenValidationParameters.TokenReplayCache was unable to add the securityToken: '{0}'.";
        internal const string IDX10233 = "IDX10233: ValidateAudience property on ValidationParameters is set to false. Exiting without validating the audience.";
        internal const string IDX10234 = "IDX10244: Audience Validated.Audience: '{0}'";
        internal const string IDX10235 = "IDX10235: ValidateIssuer property on ValidationParameters is set to false. Exiting without validating the issuer.";
        internal const string IDX10236 = "IDX10236: Issuer Validated.Issuer: '{0}'";
        internal const string IDX10237 = "IDX10237: ValidateIssuerSigningKey property on ValidationParameters is set to false. Exiting without validating the issuer signing key.";
        internal const string IDX10238 = "IDX10238: ValidateLifetime property on ValidationParameters is set to false. Exiting without validating the lifetime.";
        internal const string IDX10239 = "IDX10239: Lifetime of the token is valid.";
        internal const string IDX10240 = "IDX10240: No token replay is detected.";
        internal const string IDX10245 = "IDX10245: Creating claims identity from the validated token: '{0}'.";

        // Formating
        internal const string IDX14700 = "IDX14700: Unable to decode: '{0}' as Base64url encoded string.";

        // Crypto Errors
        internal const string IDX10600 = "IDX10600: '{0}' supports: '{1}' of types: '{2}' or '{3}'. SecurityKey received was of type '{4}'.";
        internal const string IDX10603 = "IDX10603: The algorithm: '{0}' requires the SecurityKey.KeySize to be greater than '{1}' bits. KeySize reported: '{2}'.";
        internal const string IDX10613 = "IDX10613: Cannot set the MinimumAsymmetricKeySizeInBitsForSigning to less than '{0}'.";
        internal const string IDX10623 = "IDX10623: Cannot sign data because the KeyedHashAlgorithm is null.";
        internal const string IDX10624 = "IDX10624: Cannot verify data because the KeyedHashAlgorithm is null.";
        internal const string IDX10627 = "IDX10627: Cannot set the MinimumAsymmetricKeySizeInBitsForVerifying to less than '{0}'.";
        internal const string IDX10628 = "IDX10628: Cannot set the MinimumSymmetricKeySizeInBits to less than '{0}'.";
        internal const string IDX10630 = "IDX10630: The '{0}' for signing cannot be smaller than '{1}' bits. KeySize: '{2}'.";
        internal const string IDX10631 = "IDX10631: The '{0}' for verifying cannot be smaller than '{1}' bits. KeySize: '{2}'.";
        internal const string IDX10634 = "IDX10634: Unable to create the SignatureProvider.\nAlgorithm: '{0}', SecurityKey: '{1}'\n is not supported.";
        internal const string IDX10638 = "IDX10638: Cannot created the SignatureProvider, 'key.HasPrivateKey' is false, cannot create signatures. Key: {0}.";
        internal const string IDX10640 = "IDX10640: Algorithm is not supported: '{0}'.";
        internal const string IDX10641 = "IDX10641: Key is not supported: '{0}'.";
        internal const string IDX10642 = "IDX10642: Creating signature using the input: '{0}'.";
        internal const string IDX10643 = "IDX10643: Comparing the signature created over the input with the token signature: '{0}'.";
        internal const string IDX10644 = "IDX10644: Crypto operation not supported.";
        internal const string IDX10645 = "IDX10645: Elliptical Curve not supported for curveId: '{0}'";
        internal const string IDX10646 = "IDX10646: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}', Key: '{1}'), but Create.(algorithm, args) as '{2}' == NULL.";
        internal const string IDX10647 = "IDX10647: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}'), but Create.(algorithm, args) as '{1}' == NULL.";
        internal const string IDX10648 = "IDX10648: The SecurityKey provided for AuthenticatedEncryption must be a SymmetricSecurityKey. Type is: '{0}'.";
        internal const string IDX10649 = "IDX10649: Failed to create a SymmetricSignatureProvider for the algorithm '{0}'.";
        internal const string IDX10650 = "IDX10650: Failed to verify ciphertext with aad '{0}'; iv '{1}'; and authenticationTag '{2}'.";
        internal const string IDX10651 = "IDX10651: The key length for the algorithm '{0]' cannot be less than '{1}'.";
        internal const string IDX10652 = "IDX10652: The algorithm '{0}' is not supported.";
        internal const string IDX10653 = "IDX10653: The encryption algorithm '{0}' requires a key size of at least '{1}' bits. Key '{2}', is of size: '{3}'.";
        internal const string IDX10654 = "IDX10654: Decryption failed. Crypto operation exception: '{0}'.";
        internal const string IDX10655 = "IDX10655: 'length' must be greater than 1: '{0}'";
        internal const string IDX10656 = "IDX10656: 'length' cannot be greater than signature.Length. length: '{0}', signature.Length: '{1}'.";
        internal const string IDX10657 = "IDX10657: The SecurityKey provided for the symmetric key wrap algorithm cannot be converted to byte array. Type is: '{0}'.";
        internal const string IDX10658 = "IDX10658: WrapKey failed, exception from crypto operation: '{0}'";
        internal const string IDX10659 = "IDX10659: UnwrapKey failed, exception from crypto operation: '{0}'";
        internal const string IDX10660 = "IDX10660: The Key: '{0}' and algorithm: '{1}' pair are not supported.";
        internal const string IDX10661 = "IDX10661: Unable to create the KeyWrapProvider.\nKeyWrapAlgorithm: '{0}', SecurityKey: '{1}'\n is not supported.";
        internal const string IDX10662 = "IDX10662: The KeyWrap algorithm '{0}' requires a key size of '{1}' bits. Key '{2}', is of size:'{3}'.";
        internal const string IDX10663 = "IDX10663: Failed to create symmetric algorithm with SecurityKey: '{0}', KeyWrapAlgorithm: '{1}'.";
        internal const string IDX10664 = "IDX10664: The length of input must be a multiple of 64 bits. The input size is: '{0}' bits.";
        internal const string IDX10665 = "IDX10665: Data is not authentic";
        internal const string IDX10666 = "IDX10666: Unable to create KeyedHashAlgorithm for algorithm '{0}'.";
        internal const string IDX10667 = "IDX10667: Unable to obtain required byte array for KeyHashAlgorithm from SecurityKey: '{0}'.";
        internal const string IDX10668 = "IDX10668: Unable to create '{0}', algorithm '{1}'; key: '{2}' is not supported.";
        internal const string IDX10669 = "IDX10669: Failed to create symmetric algorithm.";
        internal const string IDX10670 = "IDX10670: The lengths of the two byte arrays do not match. The first one has: '{0}' bytes, the second one has: '{1}' bytes.";
        internal const string IDX10671 = "IDX10671: The ECDsa Key: '{0}' must be '{1}' bits. KeySize: '{2}'.";

        // security keys
        internal const string IDX10700 = "IDX10700: Invalid RsaParameters: '{0}'. Both modulus and exponent should be present";
        internal const string IDX10701 = "IDX10701: Invalid JsonWebKey rsa keying material: '{0}'. Both modulus and exponent should be present";
        internal const string IDX10702 = "IDX10702: One or more private RSA key parts are null in the JsonWebKey: '{0}'";
        internal const string IDX10703 = "IDX10703: Cannot create symmetric security key. Key length is zero.";

        // Json specific errors
        internal const string IDX10801 = "IDX10801: Unable to create an RSA public key from the Exponent and Modulus found in the JsonWebKey: E: '{0}', N: '{1}'. See inner exception for additional details.";
        internal const string IDX10802 = "IDX10802: Unable to create an X509Certificate2 from the X509Data: '{0}'. See inner exception for additional details.";
        internal const string IDX10804 = "IDX10804: Unable to retrieve document from: '{0}'.";
        internal const string IDX10805 = "IDX10805: Error deserializing json: '{0}' into '{1}'.";
        internal const string IDX10806 = "IDX10806: Deserializing json: '{0}' into '{1}'.";
#pragma warning restore 1591
    }
}
