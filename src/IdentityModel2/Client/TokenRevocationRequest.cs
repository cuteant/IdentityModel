﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace CuteAnt.IdentityModel.Client
{
    public class TokenRevocationRequest
    {
        public string Token { get; set; }
        public string TokenTypeHint { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public TokenRevocationRequest()
        {
            Token = "";
            ClientId = "";
            ClientSecret = "";
        }
    }
}