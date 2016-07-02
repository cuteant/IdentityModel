﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
#if NET40
using CuteAnt.Text.Encodings.Web;
#else
using System.Text.Encodings.Web;
#endif

namespace IdentityModel.Client
{
  public class AuthorizeRequest
  {
    private readonly Uri _authorizeEndpoint;

    public AuthorizeRequest(Uri authorizeEndpoint)
    {
      _authorizeEndpoint = authorizeEndpoint;
    }

    public AuthorizeRequest(string authorizeEndpoint)
    {
      _authorizeEndpoint = new Uri(authorizeEndpoint);
    }

    public string Create(IDictionary<string, string> values)
    {
      // ## 苦竹 修改 ##
      //var qs = string.Join("&", values.Select(kvp => String.Format("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value))).ToArray());
      var qs = string.Join("&", values.Select(kvp => String.Format("{0}={1}", UrlEncoder.Default.Encode(kvp.Key), UrlEncoder.Default.Encode(kvp.Value))).ToArray());
      return string.Format("{0}?{1}", _authorizeEndpoint.AbsoluteUri, qs);
    }
  }
}