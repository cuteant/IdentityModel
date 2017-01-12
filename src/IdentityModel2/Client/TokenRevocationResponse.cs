﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace CuteAnt.IdentityModel.Client
{
    public class TokenRevocationResponse
    {
        public string Raw { get; }
        public JObject Json { get; }
        public bool IsError { get; }
        public HttpStatusCode HttpStatusCode { get; }
        public string HttpErrorReason { get; }
        public ResponseErrorType ErrorType { get;  }
        public Exception Exception { get;  }

        public TokenRevocationResponse()
        {
            IsError = false;
            HttpStatusCode = HttpStatusCode.OK;
        }

        public TokenRevocationResponse(string raw)
        {
            Raw = raw;
            IsError = false;
            HttpStatusCode = HttpStatusCode.OK;

            try
            {
                Json = JObject.Parse(raw);

                if (!string.IsNullOrEmpty(Json.TryGetString(OidcConstants.TokenResponse.Error)))
                {
                    IsError = true;
                    ErrorType = ResponseErrorType.Protocol;
                    HttpStatusCode = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorType = ResponseErrorType.Exception;
                Exception = ex;
            }
        }

        public TokenRevocationResponse(HttpStatusCode statusCode, string reason)
        {
            IsError = true;

            ErrorType = ResponseErrorType.Http;
            HttpStatusCode = statusCode;
            HttpErrorReason = reason;
        }

        public TokenRevocationResponse(Exception exception)
        {
            IsError = true;

            Exception = exception;
            ErrorType = ResponseErrorType.Exception;
        }

        public string Error
        {
            get
            {
                if (ErrorType == ResponseErrorType.Http)
                {
                    return HttpErrorReason;
                }
                else if(ErrorType == ResponseErrorType.Exception)
                {
                    return Exception.Message;
                }

                return Json.TryGetString(OidcConstants.TokenResponse.Error);
            }
        }
    }
}