// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Jwk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET40
using CuteAnt.Extensions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace IdentityModel.HttpSigning
{
    public class Cnf
    {
        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public Cnf(JsonWebKey jwk)
        {
            this.jwk = jwk;
        }

        public JsonWebKey jwk { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, _jsonSettings);
        }

      
    }

    public class CnfParser
    {
        private static readonly ILogger Logger = TraceLogger.LoggerFactory.GetCurrentClassLogger();

        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static JsonWebKey Parse(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            Cnf cnf = null;
            try
            {
                cnf = JsonConvert.DeserializeObject<Cnf>(json, _jsonSettings);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to parse CNF JSON");
            }

            if (cnf == null)
            {
                Logger.LogError("cnf JSON failed to parse");
            }
            else if (cnf.jwk == null)
            {
                Logger.LogError("jwk missing in cnf");
            }

            return cnf?.jwk;
        }
    }
}
