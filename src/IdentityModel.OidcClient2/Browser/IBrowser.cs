﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;

namespace CuteAnt.IdentityModel.OidcClient.Browser
{
    public interface IBrowser
    {
        Task<BrowserResult> InvokeAsync(BrowserOptions options);
    }
}