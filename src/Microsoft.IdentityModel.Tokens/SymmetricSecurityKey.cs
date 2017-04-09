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

using System;
using Microsoft.IdentityModel.Logging;
using System.Globalization;

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// Represents a symmetric security key.
    /// </summary>
    public class SymmetricSecurityKey : SecurityKey
    {
        int _keySize;
        byte[] _key;

        /// <summary>
        /// Returns a new instance of <see cref="SymmetricSecurityKey"/> instance.
        /// </summary>
        /// <param name="key">The byte array of the key.</param>
        public SymmetricSecurityKey(byte[] key)
        {
            if (key == null)
                throw LogHelper.LogArgumentNullException(nameof(key));

            if (key.Length == 0)
                throw LogHelper.LogExceptionMessage(new ArgumentException(LogMessages.IDX10703));

            _key = key.CloneByteArray();
            _keySize = _key.Length * 8;
        }

        /// <summary>
        /// Gets the key size.
        /// </summary>
        public override int KeySize
        {
            get { return _keySize; }
        }

        /// <summary>
        /// Gets the byte array of the key.
        /// </summary>
        public virtual byte[] Key
        {
            get { return _key.CloneByteArray(); }
        }
    }
}
