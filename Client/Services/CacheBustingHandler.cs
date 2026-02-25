using System;

namespace TradeUp.Client.Services
{
    public class CacheBustingHandler : DelegatingHandler
    {
        private readonly string _version;

        public CacheBustingHandler(IConfiguration config)
        {
            _version = config["AppSettings:Version"] ?? DateTime.Now.Ticks.ToString();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                var uri = request.RequestUri!.ToString();

                if (uri.Contains("."))
                {
                    var connector = uri.Contains("?") ? "&" : "?";
                    request.RequestUri = new Uri($"{uri}{connector}v={_version}");
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
