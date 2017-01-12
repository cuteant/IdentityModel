﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace CuteAnt.IdentityModel.Client
{
    public class TokenResponse
    {
        public TokenResponse(string raw)
        {
            Raw = raw;

            try
            {
                Json = JObject.Parse(raw);
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorType = ResponseErrorType.Exception;
                Exception = ex;

                return;
            }

            if (string.IsNullOrWhiteSpace(Error))
            {
                IsError = false;
                HttpStatusCode = HttpStatusCode.OK;
            }
            else
            {
                IsError = true;
                HttpStatusCode = HttpStatusCode.BadRequest;
                ErrorType = ResponseErrorType.Protocol;
            }
        }
        
        public TokenResponse(Exception exception)
        {
            IsError = true;

            Exception = exception;
            ErrorType = ResponseErrorType.Exception;
        }

        public TokenResponse(HttpStatusCode statusCode, string reason)
        {
            IsError = true;

            ErrorType = ResponseErrorType.Http;
            HttpStatusCode = statusCode;
            HttpErrorReason = reason;
        }

        public string Raw { get; }
        public JObject Json { get; }
        public Exception Exception { get; set; }

        public bool IsError { get; }
        public ResponseErrorType ErrorType { get; } = ResponseErrorType.None;
        public HttpStatusCode HttpStatusCode { get; }
        public string HttpErrorReason { get; }

        public string AccessToken => TryGet(OidcConstants.TokenResponse.AccessToken);
        public string IdentityToken => TryGet(OidcConstants.TokenResponse.IdentityToken);
        public string TokenType => TryGet(OidcConstants.TokenResponse.TokenType);
        public string RefreshToken => TryGet(OidcConstants.TokenResponse.RefreshToken);
        public string ErrorDescription => TryGet(OidcConstants.TokenResponse.ErrorDescription);

        public long ExpiresIn
        {
            get
            {
                var value = TryGet(OidcConstants.TokenResponse.ExpiresIn);

                if (value != null)
                {
                    long longValue;
                    if (long.TryParse(value.ToString(), out longValue))
                    {
                        return longValue;
                    }
                }

                return 0;
            }
        }

        public string Error
        {
            get
            {
                if (ErrorType == ResponseErrorType.Http)
                {
                    return HttpErrorReason;
                }
                else if (ErrorType == ResponseErrorType.Exception)
                {
                    return Exception.Message;
                }

                return TryGet(OidcConstants.TokenResponse.Error);
            }
        }

        public string TryGet(string name) => Json.TryGetString(name);
    }
}