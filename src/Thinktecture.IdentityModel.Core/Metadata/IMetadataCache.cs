/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System;

namespace Thinktecture.IdentityModel.Metadata
{
    public interface IMetadataCache
    {
        TimeSpan Age { get; }
        byte[] Load();
        void Save(byte[] data);
    }
}