// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CuteAnt.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IdentityModel.HttpSigning
{
    public class EncodedParameters
    {
        private static readonly ILogger Logger = TraceLogger.LoggerFactory.GetCurrentClassLogger();

        public EncodedParameters(string accessToken)
        {
            if (String.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            AccessToken = accessToken;
        }

        public EncodedParameters(IDictionary<string, object> values)
        {
            if (values == null) throw new ArgumentNullException("values");

            Decode(values);
        }

        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static EncodedParameters FromJson(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
            {
                Logger.LogError("No JSON");
                return null;
            }

            Dictionary<string, object> values = null;
            try
            {
                values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, _jsonSettings);
                return new EncodedParameters(values);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Failed to deserialize JSON");
            }

            return null;
        }

        public string AccessToken { get; private set; }
        public long? TimeStamp { get; set; }
        public string Method { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public EncodedList QueryParameters { get; set; }
        public EncodedList RequestHeaders { get; set; }
        public string BodyHash { get; set; }

        public bool IsSame(EncodedParameters other)
        {
            if (other == null) return false;

            if (AccessToken != other.AccessToken)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("AccessToken mismatch");
                return false;
            }
            if (Method != other.Method)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Method mismatch");
                return false;
            }
            if (Host != other.Host)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Host mismatch");
                return false;
            }
            if (Path != other.Path)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Path mismatch");
                return false;
            }
            if (BodyHash != other.BodyHash)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("BodyHash mismatch");
                return false;
            }

            if (QueryParameters == null && other.QueryParameters != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("One QueryParameters is null, the other is not");
                return false;
            }
            if (QueryParameters != null && other.QueryParameters == null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("One QueryParameters is null, the other is not");
                return false;
            }
            if (QueryParameters != null && !QueryParameters.IsSame(other.QueryParameters))
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("QueryParameters mismatch");
                return false;
            }

            if (RequestHeaders == null && other.RequestHeaders != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("One RequestHeaders is null, the other is not");
                return false;
            }
            if (RequestHeaders != null && other.RequestHeaders == null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("One RequestHeaders is null, the other is not");
                return false;
            }
            if (RequestHeaders != null && !RequestHeaders.IsSame(other.RequestHeaders))
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("RequestHeaders mismatch");
                return false;
            }

            return true;
        }

        public Dictionary<string, object> Encode()
        {
            var value = new Dictionary<string, object>();

            value.Add(HttpSigningConstants.SignedObjectParameterNames.AccessToken, AccessToken);

            if (TimeStamp != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Encoding timestamp");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.TimeStamp, TimeStamp.Value);
            }

            if (Method != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Encoding method");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Method, Method);
            }

            if (Host != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Encoding host");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Host, Host);
            }

            if (Path != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Encoding path");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Path, Path);
            }

            if (QueryParameters != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Encoding query params");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedQueryParameters, QueryParameters.Encode());
            }

            if (RequestHeaders != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Encoding request headers");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedRequestHeaders, RequestHeaders.Encode());
            }

            if (BodyHash != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Encoding body hash");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedRequestBody, BodyHash);
            }

            return value;
        }

        private void Decode(IDictionary<string, object> values)
        {
            AccessToken = GetString(values, HttpSigningConstants.SignedObjectParameterNames.AccessToken);
            if (AccessToken == null)
            {
                Logger.LogError(HttpSigningConstants.SignedObjectParameterNames.AccessToken + " value not present");
                throw new ArgumentException(HttpSigningConstants.SignedObjectParameterNames.AccessToken + " value not present");
            }

            var ts = GetNumber(values, HttpSigningConstants.SignedObjectParameterNames.TimeStamp);
            if (ts != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Decoded Timestamp");
                TimeStamp = ts;
            }

            Method = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Method);
            if (Method != null) if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Decoded Method");

            Host = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Host);
            if (Host != null) if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Decoded Host");

            Path = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Path);
            if (Path != null) if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Decoded Path");

            QueryParameters = GetDecodedList(values, HttpSigningConstants.SignedObjectParameterNames.HashedQueryParameters);
            if (QueryParameters != null) if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Decoded QueryParameters");

            RequestHeaders = GetDecodedList(values, HttpSigningConstants.SignedObjectParameterNames.HashedRequestHeaders);
            if (RequestHeaders != null) if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Decoded RequestHeaders");

            BodyHash = GetString(values, HttpSigningConstants.SignedObjectParameterNames.HashedRequestBody);
            if (BodyHash != null) if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Decoded BodyHash");
        }

        EncodedList GetDecodedList(IDictionary<string, object> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var item = values[key];
                return new EncodedList(item);
            }
            return null;
        }

        string GetString(IDictionary<string, object> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var item = values[key] as string;
                if (item == null)
                {
                    Logger.LogError(key + " must be a string");
                    throw new ArgumentException(key + " must be a string");
                }
                return item;
            }
            return null;
        }

        long? GetNumber(IDictionary<string, object> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var item = values[key];
                var type = item.GetType();

                if (typeof(long) == type)
                {
                    return (long)item;
                }

                if (typeof(int) == type)
                {
                    return (int)item;
                }

                if (typeof(short) == type)
                {
                    return (short)item;
                }

                Logger.LogError(key + " must be a number");
                throw new ArgumentException(key + " must be a number");
            }
            return null;
        }
    }
}
