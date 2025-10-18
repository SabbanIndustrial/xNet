#nullable enable
using System;
using System.Net;

namespace xNet
{
    /// <summary>
    /// Provides a cross-platform implementation that relies on environment variables and <see cref="IWebProxy"/> values.
    /// </summary>
    internal sealed class CrossPlatformNetworkSettingsProvider : INetworkSettingsProvider
    {
        private static readonly Uri ProbeUri = new Uri("http://example.com");
        private string? _overrideProxyString;
        private HttpProxyClient? _overrideProxy;

        /// <inheritdoc/>
        public bool InternetConnected
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public bool InternetThroughModem
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool InternetThroughLan
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public bool InternetThroughProxy
        {
            get
            {
                return ProxyEnabled;
            }
        }

        /// <inheritdoc/>
        public bool ProxyEnabled
        {
            get
            {
                return IEProxy != null;
            }
            set
            {
                if (!value)
                {
                    _overrideProxy = null;
                    _overrideProxyString = null;
                }
            }
        }

        /// <inheritdoc/>
        public HttpProxyClient? IEProxy
        {
            get
            {
                HttpProxyClient? proxy = GetOverrideProxy();

                if (proxy != null)
                {
                    return proxy;
                }

                proxy = ResolveProxyFromEnvironment();

                if (proxy != null)
                {
                    return proxy;
                }

                return ResolveProxyFromDefaultWebProxy();
            }
            set
            {
                _overrideProxy = value;
                _overrideProxyString = value != null ? value.ToString() : null;
            }
        }

        /// <inheritdoc/>
        public string GetProxyString()
        {
            if (!string.IsNullOrEmpty(_overrideProxyString))
            {
                return _overrideProxyString!;
            }

            HttpProxyClient? proxy = IEProxy;

            return proxy != null ? proxy.ToString() : string.Empty;
        }

        /// <inheritdoc/>
        public void SetProxyString(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _overrideProxyString = null;
                _overrideProxy = null;
                return;
            }

            string trimmed = value.Trim();
            _overrideProxyString = trimmed;
            _overrideProxy = ParseProxyString(trimmed);
        }

        private HttpProxyClient? GetOverrideProxy()
        {
            if (_overrideProxy != null)
            {
                return _overrideProxy;
            }

            if (_overrideProxyString == null)
            {
                return null;
            }

            _overrideProxy = ParseProxyString(_overrideProxyString);
            return _overrideProxy;
        }

        private HttpProxyClient? ResolveProxyFromEnvironment()
        {
            string[] keys = new[] { "http_proxy", "HTTP_PROXY", "https_proxy", "HTTPS_PROXY" };

            foreach (string key in keys)
            {
                string? value = Environment.GetEnvironmentVariable(key);

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                HttpProxyClient? proxy = ParseProxyString(value.Trim());

                if (proxy != null)
                {
                    return proxy;
                }
            }

            return null;
        }

        private HttpProxyClient? ResolveProxyFromDefaultWebProxy()
        {
            IWebProxy? webProxy = WebRequest.DefaultWebProxy;

            if (webProxy == null)
            {
                return null;
            }

            if (webProxy.IsBypassed(ProbeUri))
            {
                return null;
            }

            Uri? proxyUri = webProxy.GetProxy(ProbeUri);

            if (proxyUri == null || proxyUri == ProbeUri)
            {
                return null;
            }

            return CreateProxyFromUri(proxyUri, ResolveCredentials(webProxy, proxyUri));
        }

        private static HttpProxyClient? ParseProxyString(string value)
        {
            HttpProxyClient? parsed;

            if (HttpProxyClient.TryParse(value, out parsed))
            {
                return parsed;
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out Uri? uri))
            {
                return CreateProxyFromUri(uri, ExtractCredentials(uri));
            }

            return null;
        }

        private static HttpProxyClient? CreateProxyFromUri(Uri? uri, NetworkCredential? credential)
        {
            if (uri == null || string.IsNullOrEmpty(uri.Host))
            {
                return null;
            }

            int port = uri.Port;

            if (port <= 0)
            {
                port = uri.Scheme == Uri.UriSchemeHttps ? 443 : 80;
            }

            string username = credential != null ? credential.UserName : string.Empty;
            string password = credential != null ? credential.Password : string.Empty;

            return new HttpProxyClient(uri.Host, port, username, password);
        }

        private static NetworkCredential? ExtractCredentials(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            string userInfo = uri.UserInfo;

            if (string.IsNullOrEmpty(userInfo))
            {
                return null;
            }

            string[] parts = userInfo.Split(new[] { ':' }, 2);
            string username = Uri.UnescapeDataString(parts[0]);
            string password = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;

            return new NetworkCredential(username, password);
        }

        private static NetworkCredential? ResolveCredentials(IWebProxy webProxy, Uri proxyUri)
        {
            ICredentials? credentials = webProxy.Credentials;

            if (credentials == null)
            {
                credentials = CredentialCache.DefaultCredentials;
            }

            if (credentials == null)
            {
                return null;
            }

            return credentials.GetCredential(proxyUri, "Basic");
        }
    }
}
