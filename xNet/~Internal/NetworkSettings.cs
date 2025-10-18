#nullable enable
using System;
using System.Threading;

namespace xNet
{
    /// <summary>
    /// Provides access to the network settings provider used by the library.
    /// </summary>
    public static class NetworkSettings
    {
        private static INetworkSettingsProvider _provider = CreateDefaultProvider();

        /// <summary>
        /// Gets or sets the <see cref="INetworkSettingsProvider"/> used by the library.
        /// </summary>
        public static INetworkSettingsProvider Provider
        {
            get
            {
                return Volatile.Read(ref _provider);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                Volatile.Write(ref _provider, value);
            }
        }

        /// <summary>
        /// Restores the default provider based on the current operating system.
        /// </summary>
        public static void Reset()
        {
            Provider = CreateDefaultProvider();
        }

        private static INetworkSettingsProvider CreateDefaultProvider()
        {
#if WINDOWS
            if (OperatingSystem.IsWindows())
            {
                return new WindowsNetworkSettingsProvider();
            }
#endif
            return new CrossPlatformNetworkSettingsProvider();
        }
    }
}
