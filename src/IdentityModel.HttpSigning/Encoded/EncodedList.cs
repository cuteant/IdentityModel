﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CuteAnt.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityModel.HttpSigning
{
    public class EncodedList
    {
        private static readonly ILogger Logger = TraceLogger.LoggerFactory.GetCurrentClassLogger();

        public EncodedList(IEnumerable<string> keys, string hashValue)
        {
            if (keys == null || !keys.Any()) throw new ArgumentNullException("keys");
            if (String.IsNullOrWhiteSpace(hashValue)) throw new ArgumentNullException("hashValue");

            this.Keys = keys;
            this.HashedValue = hashValue;
        }

        public EncodedList(object list)
        {
            if (list == null) throw new ArgumentNullException("list");

            var collection = list as IEnumerable<object>;
            if (collection == null)
            {
                Logger.LogError("list is not an array");
                throw new ArgumentException("list is not an array");
            }

            object[] arr = collection.ToArray();
            Decode(arr);
        }

        public bool IsSame(EncodedList other)
        {
            if (other == null) return false;

            var arr1 = Keys.ToArray();
            var arr2 = other.Keys.ToArray();

            var len = arr1.Length;
            if (arr1.Length != arr2.Length)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Keys are not same length");
                return false;
            }

            if (HashedValue != other.HashedValue)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("HashedValues do not match");
                return false;
            }

            for (var i = 0; i < Keys.Count(); i++)
            {
                if (arr1[i] != arr2[i])
                {
                    if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Values at position {0} do not match", i);
                    return false;
                }
            }

            return true;
        }

        private void Decode(object[] arr)
        {
            if (arr.Length != 2)
            {
                Logger.LogError("list does not have exactly two items");
                throw new ArgumentException("list does not have exactly two items");
            }

            var items = arr[0] as IEnumerable<object>;
            if (items == null)
            {
                Logger.LogError("first item in list is not array of strings");
                throw new ArgumentException("first item in list is not array of strings");
            }

            var keys = items.Select(x => Convert.ToString(x)).ToArray();
            if (keys.Length != items.Count())
            {
                Logger.LogError("key count don't match item count");
                throw new ArgumentException("key count don't match item count");
            }

            if (arr[1] == null)
            {
                Logger.LogError("second item in list is not a string");
                throw new ArgumentException("second item in list is not a string");
            }

            var value = Convert.ToString(arr[1]);
            if (value == null)
            {
                Logger.LogError("second item in list is not a string");
                throw new ArgumentException("second item in list is not a string");
            }

            Keys = keys;
            HashedValue = value;
        }

        public object[] Encode()
        {
            return new object[]
            {
                Keys, HashedValue
            };
        }

        public IEnumerable<string> Keys { get; private set; }
        public string HashedValue { get; private set; }
    }
}
