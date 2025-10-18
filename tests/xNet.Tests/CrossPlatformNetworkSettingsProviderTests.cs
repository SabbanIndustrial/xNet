#nullable enable
using System;
using System.Net;
using Xunit;

namespace xNet.Tests
{
    public class CrossPlatformNetworkSettingsProviderTests
    {
        [Fact]
        public void UsesEnvironmentVariableForProxy()
        {
            string[] keys = new[] { "http_proxy", "HTTP_PROXY", "https_proxy", "HTTPS_PROXY" };
            string?[] originals = CaptureEnvironment(keys);
            IWebProxy? originalProxy = WebRequest.DefaultWebProxy;
            string proxyValue = "http://user:password@localhost:4321";

            try
            {
                SetEnvironment(keys, proxyValue);
                WebRequest.DefaultWebProxy = null;

                var provider = new CrossPlatformNetworkSettingsProvider();

                HttpProxyClient? proxy = provider.IEProxy;

                Assert.NotNull(proxy);
                Assert.Equal("localhost", proxy!.Host, StringComparer.OrdinalIgnoreCase);
                Assert.Equal(4321, proxy.Port);
                Assert.Equal("user", proxy.Username);
                Assert.Equal("password", proxy.Password);
                Assert.True(provider.ProxyEnabled);
                Assert.False(string.IsNullOrEmpty(provider.GetProxyString()));
            }
            finally
            {
                RestoreEnvironment(keys, originals);
                WebRequest.DefaultWebProxy = originalProxy;
            }
        }

        [Fact]
        public void SetProxyStringOverridesEnvironment()
        {
            string[] keys = new[] { "http_proxy", "HTTP_PROXY", "https_proxy", "HTTPS_PROXY" };
            string?[] originals = CaptureEnvironment(keys);
            IWebProxy? originalProxy = WebRequest.DefaultWebProxy;

            SetEnvironment(keys, null);
            WebRequest.DefaultWebProxy = null;

            try
            {
                var provider = new CrossPlatformNetworkSettingsProvider();
                string proxyString = "proxy.example.com:8080:login:secret";

                provider.SetProxyString(proxyString);

                HttpProxyClient? proxy = provider.IEProxy;

                Assert.NotNull(proxy);
                Assert.Equal("proxy.example.com", proxy!.Host, StringComparer.OrdinalIgnoreCase);
                Assert.Equal(8080, proxy.Port);
                Assert.Equal("login", proxy.Username);
                Assert.Equal("secret", proxy.Password);
                Assert.Equal(proxyString, provider.GetProxyString());

                provider.ProxyEnabled = false;
                Assert.Null(provider.IEProxy);
            }
            finally
            {
                RestoreEnvironment(keys, originals);
                WebRequest.DefaultWebProxy = originalProxy;
            }
        }

        private static string?[] CaptureEnvironment(string[] keys)
        {
            string?[] values = new string?[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                values[i] = Environment.GetEnvironmentVariable(keys[i]);
            }

            return values;
        }

        private static void SetEnvironment(string[] keys, string? value)
        {
            foreach (string key in keys)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        private static void RestoreEnvironment(string[] keys, string?[] originals)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                Environment.SetEnvironmentVariable(keys[i], originals[i]);
            }
        }
    }
}
