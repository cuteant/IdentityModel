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
using System.Xml;

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// Defines the interface for a Security Token Handler.
    /// </summary>
    public abstract class SecurityTokenHandler
    {
        /// <summary>
        /// Creates an instance of <see cref="SecurityTokenHandler"/>
        /// </summary>
        protected SecurityTokenHandler()
        {
        }

        /// <summary>
        /// Returns <see cref="SecurityKeyIdentifierClause"/>.
        /// </summary>
        /// <param name="token"><see cref="SecurityToken"/></param>
        /// <param name="attached">true if attached; otherwise, false.</param>
        public virtual SecurityKeyIdentifierClause CreateSecurityTokenReference(SecurityToken token, bool attached)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns <see cref="SecurityToken"/>.
        /// </summary>
        /// <param name="tokenDescriptor"><see cref="SecurityTokenDescriptor"/></param>
        public virtual SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether this handler supports validation of tokens 
        /// handled by this instance.
        /// </summary>v
        /// <returns>'True' if the instance is capable of SecurityToken
        /// validation.</returns>
        public virtual bool CanValidateToken
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the class provides serialization functionality to serialize token handled 
        /// by this instance.
        /// </summary>
        /// <returns>true if the WriteToken method can serialize this token.</returns>
        public virtual bool CanWriteToken
        {
            get { return false; }
        }

        /// <summary>
        /// This must be overridden to get the System.Type of the SecurityToken this instance handles.
        /// </summary>
        public abstract Type TokenType
        {
            get;
        }

        /// <summary>
        /// Indicates whether the current token string can be read as a token 
        /// of the type handled by this instance.
        /// </summary>
        /// <param name="tokenString">The token string thats needs to be read.</param>
        /// <returns>'True' if the ReadToken method can parse the token string.</returns>
        public virtual bool CanReadToken(string tokenString)
        {
            return false;
        }
        
        /// <summary>
        /// Deserializes from string a token of the type handled by this instance.
        /// </summary>
        /// <param name="tokenString">The string to be deserialized.</param>
        /// <returns>SecurityToken instance which represents the serialized token.</returns>
        public virtual SecurityToken ReadToken(string tokenString)
        {
            return null;
        }

        /// <summary>
        /// Gets security token.
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/>.</param>
        /// <remarks>SecurityToken instance which represents the serialized token.</remarks>
        public virtual SecurityToken ReadToken(XmlReader reader)
        {
            return null;
        }

        /// <summary>
        /// Serializes to string a token of the type handled by this instance.
        /// </summary>
        /// <param name="token">A token of type TokenType.</param>
        /// <returns>The serialized token.</returns>
        public virtual string WriteToken(SecurityToken token)
        {
            return null;
        }

        /// <summary>
        /// This must be overridden to serialize to XML a token of the type handled by this instance.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        /// <param name="token">A token of type <see cref="TokenType"/>.</param>
        public abstract void WriteToken(XmlWriter writer, SecurityToken token);

        /// <summary>
        /// This must be overridden to deserialize token with the provided <see cref="TokenValidationParameters"/>.
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/>.</param>
        /// <param name="validationParameters">the current <see cref="TokenValidationParameters"/>.</param>
        /// <remarks>SecurityToken instance which represents the serialized token.</remarks>
        public abstract SecurityToken ReadToken(XmlReader reader, TokenValidationParameters validationParameters);
    }
}
