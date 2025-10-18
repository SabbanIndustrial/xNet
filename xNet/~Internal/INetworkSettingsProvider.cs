#nullable enable

namespace xNet
{
    /// <summary>
    /// Provides access to operating system level network configuration values used by the library.
    /// </summary>
    public interface INetworkSettingsProvider
    {
        /// <summary>
        /// Gets a value indicating whether an internet connection is available.
        /// </summary>
        bool InternetConnected { get; }

        /// <summary>
        /// Gets a value indicating whether the internet connection is established through a modem.
        /// </summary>
        bool InternetThroughModem { get; }

        /// <summary>
        /// Gets a value indicating whether the internet connection is established through a local network.
        /// </summary>
        bool InternetThroughLan { get; }

        /// <summary>
        /// Gets a value indicating whether the internet connection is established through a proxy server.
        /// </summary>
        bool InternetThroughProxy { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the system proxy is enabled.
        /// </summary>
        bool ProxyEnabled { get; set; }

        /// <summary>
        /// Gets or sets the system proxy represented as a <see cref="HttpProxyClient"/> instance.
        /// </summary>
        HttpProxyClient? IEProxy { get; set; }

        /// <summary>
        /// Gets the system proxy value represented as a raw string, matching the WinINet storage format.
        /// </summary>
        /// <returns>The raw proxy string or an empty string if the proxy is not configured.</returns>
        string GetProxyString();

        /// <summary>
        /// Sets the system proxy value using a raw string in the WinINet storage format.
        /// </summary>
        /// <param name="value">The raw proxy string or <see langword="null"/> to clear the proxy.</param>
        void SetProxyString(string? value);
    }
}
