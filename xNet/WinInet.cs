#nullable enable
using System;

namespace xNet
{
    /// <summary>
    /// Provides backward-compatible access to the legacy WinInet API surface.
    /// </summary>
    [Obsolete("WinInet is deprecated. Use NetworkSettings.Provider instead.")]
    public static class WinInet
    {
        /// <summary>
        /// Gets a value indicating whether an internet connection is available.
        /// </summary>
        public static bool InternetConnected
        {
            get
            {
                return NetworkSettings.Provider.InternetConnected;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the internet connection is established through a modem.
        /// </summary>
        public static bool InternetThroughModem
        {
            get
            {
                return NetworkSettings.Provider.InternetThroughModem;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the internet connection is established through a local network.
        /// </summary>
        public static bool InternetThroughLan
        {
            get
            {
                return NetworkSettings.Provider.InternetThroughLan;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the internet connection is established through a proxy server.
        /// </summary>
        public static bool InternetThroughProxy
        {
            get
            {
                return NetworkSettings.Provider.InternetThroughProxy;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the system proxy is enabled.
        /// </summary>
        public static bool IEProxyEnable
        {
            get
            {
                return NetworkSettings.Provider.ProxyEnabled;
            }
            set
            {
                NetworkSettings.Provider.ProxyEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the system proxy represented as a <see cref="HttpProxyClient"/> instance.
        /// </summary>
        public static HttpProxyClient? IEProxy
        {
            get
            {
                return NetworkSettings.Provider.IEProxy;
            }
            set
            {
                NetworkSettings.Provider.IEProxy = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the system proxy is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if the proxy is enabled; otherwise, <see langword="false"/>.</returns>
        public static bool GetIEProxyEnable()
        {
            return NetworkSettings.Provider.ProxyEnabled;
        }

        /// <summary>
        /// Sets a value indicating whether the system proxy is enabled.
        /// </summary>
        /// <param name="enabled">The new proxy state.</param>
        public static void SetIEProxyEnable(bool enabled)
        {
            NetworkSettings.Provider.ProxyEnabled = enabled;
        }

        /// <summary>
        /// Gets the raw proxy string stored by the operating system.
        /// </summary>
        /// <returns>The proxy string or an empty string if the proxy is not configured.</returns>
        public static string GetIEProxy()
        {
            return NetworkSettings.Provider.GetProxyString();
        }

        /// <summary>
        /// Sets the proxy server using host and port values.
        /// </summary>
        /// <param name="host">The proxy host.</param>
        /// <param name="port">The proxy port.</param>
        public static void SetIEProxy(string host, int port)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (host.Length == 0)
            {
                throw ExceptionHelper.EmptyString("host");
            }

            if (!ExceptionHelper.ValidateTcpPort(port))
            {
                throw ExceptionHelper.WrongTcpPort("port");
            }

            NetworkSettings.Provider.IEProxy = new HttpProxyClient(host, port);
        }

        /// <summary>
        /// Sets the proxy server using a raw proxy string.
        /// </summary>
        /// <param name="hostAndPort">The proxy string in a WinINet compatible format.</param>
        public static void SetIEProxy(string hostAndPort)
        {
            NetworkSettings.Provider.SetProxyString(hostAndPort);
        }
    }
}
