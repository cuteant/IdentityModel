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

namespace Microsoft.IdentityModel.Protocols.OpenIdConnect
{
    /// <summary>
    /// Specific scope values that are interesting to OpenID Connect.  See http://openid.net/specs/openid-connect-messages-1_0.html#scopes
    /// </summary>
    /// <remarks>Can be used to determine the scope by consumers of an <see cref="OpenIdConnectMessage"/>.
    /// For example: OpenIdConnectMessageTests.Publics() sets <see cref="OpenIdConnectMessage.Scope"/>
    /// to <see cref="OpenIdConnectScope.OpenIdProfile"/>.</remarks>
    public static class OpenIdConnectScope
    {
        /// <summary>
        /// Indicates openid scope see: http://openid.net/specs/openid-connect-core-1_0.html#ScopeClaims.
        /// </summary>
        public const string OpenId = "openid";

        /// <summary>
        /// Indicates openid profile scope see: http://openid.net/specs/openid-connect-core-1_0.html#ScopeClaims.
        /// </summary>
        public const string OpenIdProfile = "openid profile";

        /// <summary>
        /// Indicates user_impersonation scope see: http://openid.net/specs/openid-connect-core-1_0.html#ScopeClaims.
        /// </summary>
        public const string UserImpersonation = "user_impersonation";
    }
}
