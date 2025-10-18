#nullable enable
using System;
using Xunit;

namespace xNet.Tests
{
    public class NetworkSettingsTests
    {
        [Fact]
        public void ProviderCanBeReplacedAndReset()
        {
            INetworkSettingsProvider original = NetworkSettings.Provider;

            try
            {
                var replacement = new FakeProvider();
                NetworkSettings.Provider = replacement;

                Assert.Same(replacement, NetworkSettings.Provider);

                NetworkSettings.Reset();

#if WINDOWS
                if (OperatingSystem.IsWindows())
                {
                    Assert.Equal("WindowsNetworkSettingsProvider", NetworkSettings.Provider.GetType().Name);
                }
                else
                {
                    Assert.IsType<CrossPlatformNetworkSettingsProvider>(NetworkSettings.Provider);
                }
#else
                Assert.IsType<CrossPlatformNetworkSettingsProvider>(NetworkSettings.Provider);
#endif
            }
            finally
            {
                NetworkSettings.Provider = original;
            }
        }

        private sealed class FakeProvider : INetworkSettingsProvider
        {
            public bool InternetConnected { get; set; }

            public bool InternetThroughModem { get; set; }

            public bool InternetThroughLan { get; set; }

            public bool InternetThroughProxy { get; set; }

            public bool ProxyEnabled { get; set; }

            public HttpProxyClient? IEProxy { get; set; }

            public string GetProxyString()
            {
                return IEProxy != null ? IEProxy.ToString() : string.Empty;
            }

            public void SetProxyString(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    IEProxy = null;
                    return;
                }

                HttpProxyClient? proxy;
                if (HttpProxyClient.TryParse(value, out proxy))
                {
                    IEProxy = proxy;
                }
            }
        }
    }
}
