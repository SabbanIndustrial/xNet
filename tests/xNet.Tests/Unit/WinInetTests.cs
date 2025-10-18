using System;
using Xunit;

namespace xNet.Tests.Unit
{
    public class WinInetTests : IDisposable
    {
        public WinInetTests()
        {
            WinInet.ResetConnectionStateProvider();
        }

        [Fact]
        public void InternetThroughLan_UsesInjectedState()
        {
            WinInet.ConnectionStateProvider = () =>
            {
                var state = SafeNativeMethods.InternetConnectionState.INTERNET_CONNECTION_LAN;
                return (true, state);
            };

            Assert.True(WinInet.InternetConnected);
            Assert.True(WinInet.InternetThroughLan);
            Assert.False(WinInet.InternetThroughModem);
        }

        [Fact]
        public void InternetThroughProxy_ReturnsFalseWhenFlagMissing()
        {
            WinInet.ConnectionStateProvider = () => (false, 0);

            Assert.False(WinInet.InternetThroughProxy);
        }

        public void Dispose()
        {
            WinInet.ResetConnectionStateProvider();
        }
    }
}
