using System;
using System.IO;
using System.Security;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace xNet
{
    /// <summary>
    /// Provides backward-compatible access to the legacy WinInet API surface.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class WinInet
    {
        private const string PathToInternetOptions = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
        private static Func<(bool isConnected, SafeNativeMethods.InternetConnectionState state)> _connectionStateProvider = GetConnectionState;


        #region Статические свойства (открытые)

        /// <summary>
        /// Gets a value indicating whether an internet connection is available.
        /// </summary>
        public static bool InternetConnected
        {
            get
            {
                var snapshot = _connectionStateProvider();
                return snapshot.isConnected;
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

        internal static Func<(bool isConnected, SafeNativeMethods.InternetConnectionState state)> ConnectionStateProvider
        {
            get => _connectionStateProvider;
            set => _connectionStateProvider = value ?? throw new ArgumentNullException(nameof(value));
        }

        internal static void ResetConnectionStateProvider()
        {
            _connectionStateProvider = GetConnectionState;
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
            using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(PathToInternetOptions))
            {
                regKey.SetValue("ProxyServer", hostAndPort ?? string.Empty);
            }
        }

        #endregion


        private static bool EqualConnectedState(SafeNativeMethods.InternetConnectionState expected)
        {
            var snapshot = _connectionStateProvider();

            return (snapshot.state & expected) != 0;
        }

        private static (bool isConnected, SafeNativeMethods.InternetConnectionState state) GetConnectionState()
        {
            SafeNativeMethods.InternetConnectionState state = 0;
            bool connected = SafeNativeMethods.InternetGetConnectedState(ref state, 0);

            return (connected, state);
        }
    }
}
