#nullable enable
#if WINDOWS
using System;
using System.IO;
using System.Security;
using Microsoft.Win32;

namespace xNet
{
    /// <summary>
    /// Windows specific implementation that relies on WinINet and registry access.
    /// </summary>
    internal sealed class WindowsNetworkSettingsProvider : INetworkSettingsProvider
    {
        private const string PathToInternetOptions = @"Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";

        /// <inheritdoc/>
        public bool InternetConnected
        {
            get
            {
                if (!OperatingSystem.IsWindows())
                {
                    return false;
                }

                SafeNativeMethods.InternetConnectionState state = 0;
                return SafeNativeMethods.InternetGetConnectedState(ref state, 0);
            }
        }

        /// <inheritdoc/>
        public bool InternetThroughModem
        {
            get
            {
                return EqualConnectedState(SafeNativeMethods.InternetConnectionState.INTERNET_CONNECTION_MODEM);
            }
        }

        /// <inheritdoc/>
        public bool InternetThroughLan
        {
            get
            {
                return EqualConnectedState(SafeNativeMethods.InternetConnectionState.INTERNET_CONNECTION_LAN);
            }
        }

        /// <inheritdoc/>
        public bool InternetThroughProxy
        {
            get
            {
                return EqualConnectedState(SafeNativeMethods.InternetConnectionState.INTERNET_CONNECTION_PROXY);
            }
        }

        /// <inheritdoc/>
        public bool ProxyEnabled
        {
            get
            {
                if (!OperatingSystem.IsWindows())
                {
                    return false;
                }

                try
                {
                    return GetIEProxyEnable();
                }
                catch (IOException) { return false; }
                catch (SecurityException) { return false; }
                catch (ObjectDisposedException) { return false; }
                catch (UnauthorizedAccessException) { return false; }
            }
            set
            {
                if (!OperatingSystem.IsWindows())
                {
                    return;
                }

                try
                {
                    SetIEProxyEnable(value);
                }
                catch (IOException) { }
                catch (SecurityException) { }
                catch (ObjectDisposedException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        /// <inheritdoc/>
        public HttpProxyClient? IEProxy
        {
            get
            {
                if (!OperatingSystem.IsWindows())
                {
                    return null;
                }

                string proxyValue;

                try
                {
                    proxyValue = GetIEProxy();
                }
                catch (IOException) { return null; }
                catch (SecurityException) { return null; }
                catch (ObjectDisposedException) { return null; }
                catch (UnauthorizedAccessException) { return null; }

                HttpProxyClient? proxy;
                HttpProxyClient.TryParse(proxyValue, out proxy);

                return proxy;
            }
            set
            {
                if (!OperatingSystem.IsWindows())
                {
                    return;
                }

                try
                {
                    if (value != null)
                    {
                        SetIEProxy(value.ToString());
                    }
                    else
                    {
                        SetIEProxy(string.Empty);
                    }
                }
                catch (SecurityException) { }
                catch (ObjectDisposedException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        /// <inheritdoc/>
        public string GetProxyString()
        {
            if (!OperatingSystem.IsWindows())
            {
                return string.Empty;
            }

            try
            {
                return GetIEProxy();
            }
            catch (IOException) { return string.Empty; }
            catch (SecurityException) { return string.Empty; }
            catch (ObjectDisposedException) { return string.Empty; }
            catch (UnauthorizedAccessException) { return string.Empty; }
        }

        /// <inheritdoc/>
        public void SetProxyString(string? value)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            try
            {
                SetIEProxy(value ?? string.Empty);
            }
            catch (SecurityException) { }
            catch (ObjectDisposedException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static bool EqualConnectedState(SafeNativeMethods.InternetConnectionState expected)
        {
            if (!OperatingSystem.IsWindows())
            {
                return false;
            }

            SafeNativeMethods.InternetConnectionState state = 0;
            SafeNativeMethods.InternetGetConnectedState(ref state, 0);

            return (state & expected) != 0;
        }

        private static bool GetIEProxyEnable()
        {
            using (RegistryKey? regKey = Registry.CurrentUser.OpenSubKey(PathToInternetOptions))
            {
                if (regKey == null)
                {
                    return false;
                }

                object? value = regKey.GetValue("ProxyEnable");

                if (value == null)
                {
                    return false;
                }

                return ((int)value != 0);
            }
        }

        private static void SetIEProxyEnable(bool enabled)
        {
            using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(PathToInternetOptions)!)
            {
                regKey.SetValue("ProxyEnable", enabled ? 1 : 0);
            }
        }

        private static string GetIEProxy()
        {
            using (RegistryKey? regKey = Registry.CurrentUser.OpenSubKey(PathToInternetOptions))
            {
                if (regKey == null)
                {
                    return string.Empty;
                }

                return (regKey.GetValue("ProxyServer") as string) ?? string.Empty;
            }
        }

        private static void SetIEProxy(string value)
        {
            using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(PathToInternetOptions)!)
            {
                regKey.SetValue("ProxyServer", value ?? string.Empty);
            }
        }
    }
}
#endif
