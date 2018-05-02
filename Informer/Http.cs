using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Informer
{
    internal class Http : IDisposable
    {
        private HttpClientHandler _httpClientHandler;
        private HttpClient        _httpClient;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="httpClientHandler">Will be disposed by this class</param>
        /// <param name="httpClient">Will be disposed by this class</param>
        public Http(HttpClientHandler httpClientHandler, HttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            if (httpClientHandler == null)
                throw new ArgumentNullException(nameof(httpClientHandler));

            this._httpClient = httpClient;
            this._httpClientHandler = httpClientHandler;

        }

        public Http()
        {
            _httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,

            };
            _httpClient = new HttpClient(_httpClientHandler, false)
            {
                Timeout = TimeSpan.FromSeconds(30),
                MaxResponseContentBufferSize = 2147483647,
                DefaultRequestHeaders =
                {
                    AcceptEncoding =
                    {
                        new StringWithQualityHeaderValue("gzip"),
                        new StringWithQualityHeaderValue("deflate")
                    }
                },
            };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Informer");

        }


        /// <summary>
        /// Download string from url, return content
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetContentAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL can't be empty", nameof(url));

            return await _httpClient.GetStringAsync(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Download string from url, return content
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetContent(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL can't be empty", nameof(url));

            try {
                return _httpClient.GetStringAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                LogFile Log = new LogFile("error");
                Log.writeLogLine("Http: " + ex.Message,"error");
                return "";
            }
        }
        #region IDisposable Support
        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClientHandler?.Dispose();
                    _httpClient?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                _httpClient = null;
                _httpClientHandler = null;

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
