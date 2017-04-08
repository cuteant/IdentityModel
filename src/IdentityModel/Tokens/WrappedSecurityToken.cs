#if DESKTOPCLR && NET_4_0_GREATER
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.IdentityModel.Tokens;

namespace IdentityModel.Tokens
{
  public class WrappedSecurityToken<T> : SecurityToken
  {
    readonly T _token;

    public T Token => _token;

    public WrappedSecurityToken(T token)
    {
      _token = token;
    }

    #region Not Implemented
    public override string Id
    {
      get { throw new NotImplementedException(); }
    }

    public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys
    {
      get { throw new NotImplementedException(); }
    }

    public override DateTime ValidFrom
    {
      get { throw new NotImplementedException(); }
    }

    public override DateTime ValidTo
    {
      get { throw new NotImplementedException(); }
    }
    #endregion
  }
}
#endif