﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CuteAnt.IdentityModel.Client
{
    public class UserInfoClient
    {
        private readonly HttpClient _client;

        public UserInfoClient(string endpoint)
            : this(endpoint, new HttpClientHandler())
        { }

        public UserInfoClient(string endpoint, HttpMessageHandler innerHttpMessageHandler)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            _client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(endpoint)
            };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public TimeSpan Timeout
        {
            set
            {
                _client.Timeout = value;
            }
        }

        public async Task<UserInfoResponse> GetAsync(string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var request = new HttpRequestMessage(HttpMethod.Get, "");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new UserInfoResponse(ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                return new UserInfoResponse(response.StatusCode, response.ReasonPhrase);
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new UserInfoResponse(content);
        }
    }
}