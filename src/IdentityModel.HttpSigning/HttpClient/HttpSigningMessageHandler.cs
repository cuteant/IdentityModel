using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if NET40
using CuteAnt.Extensions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace IdentityModel.HttpSigning
{
    public class HttpSigningMessageHandler : DelegatingHandler
    {
        private static readonly ILogger Logger = TraceLogger.LoggerFactory.GetCurrentClassLogger();

        private readonly Signature _signature;
        private readonly RequestSigningOptions _options;

        public HttpSigningMessageHandler(Signature signature, RequestSigningOptions options, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            if (signature == null) throw new ArgumentNullException("signature");

            _signature = signature;
            _options = options ?? new RequestSigningOptions();
        }

        public HttpSigningMessageHandler(Signature signature, RequestSigningOptions options)
            : this(signature, options, new HttpClientHandler())
        {
        }

        public HttpSigningMessageHandler(Signature signature, HttpMessageHandler innerHandler)
            : this(signature, null, innerHandler)
        {
        }

        public HttpSigningMessageHandler(Signature signature)
            : this(signature, null, new HttpClientHandler())
        {
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await ProcessSignatureAsync(request);
            return await base.SendAsync(request, cancellationToken);
        }

        public async Task ProcessSignatureAsync(HttpRequestMessage request)
        {
            var parameters = await _options.CreateEncodingParametersAsync(request);
            if (parameters != null)
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("Encoding parameters recieved; signing and adding pop token");

                var token = _signature.Sign(parameters);
                request.AddPopToken(token);
            }
            else
            {
                if(Logger.IsDebugLevelEnabled()) Logger.LogDebug("No encoding parameters recieved; not adding pop token");
            }
        }
    }
}
