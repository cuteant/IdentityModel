// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel.HttpSigning;
using System;
#if NET40
using CuteAnt.Extensions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace IdentityModel.Jwt
{
    public static class JsonWebKeyExtensions
    {
        private static readonly ILogger Logger = TraceLogger.LoggerFactory.GetCurrentClassLogger();

        public static SigningKey ToPublicKey(this JsonWebKey key)
        {
            if (key.Kty == HttpSigningConstants.Jwk.Symmetric.KeyType)
            {
                return new SymmetricKey(key);
            }

            if (key.Kty == HttpSigningConstants.Jwk.RSA.KeyType)
            {
                return new RSAPublicKey(key);
            }

            Logger.LogError("Invalid key type: " + key.Kty);
            throw new InvalidOperationException("Invalid key type");
        }
    }
}
