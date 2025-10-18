#if NET6_0_OR_GREATER
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace xNet.Tests.Integration
{
    public sealed class TestWebHost : IAsyncDisposable
    {
        private readonly WebApplication _application;

        private TestWebHost(WebApplication application, Uri baseAddress, Uri proxyAccessibleAddress, Uri proxyIpAccessibleAddress)
        {
            _application = application;
            BaseAddress = baseAddress;
            ProxyAccessibleBaseAddress = proxyAccessibleAddress;
            ProxyAccessibleIpBaseAddress = proxyIpAccessibleAddress;
        }

        public Uri BaseAddress { get; }

        public Uri ProxyAccessibleBaseAddress { get; }

        public Uri ProxyAccessibleIpBaseAddress { get; }

        public static async Task<TestWebHost> StartAsync(Action<WebApplication> configure)
        {
            int port = GetFreePort();

            var builder = WebApplication.CreateBuilder();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, port);
            });

            var app = builder.Build();
            configure(app);
            await app.StartAsync();

            var proxyHost = Environment.GetEnvironmentVariable("XNET_PROXY_TARGET_HOST");
            if (string.IsNullOrWhiteSpace(proxyHost))
            {
                proxyHost = "127.0.0.1";
            }

            var proxyIp = Environment.GetEnvironmentVariable("XNET_PROXY_TARGET_IPV4");
            if (string.IsNullOrWhiteSpace(proxyIp))
            {
                proxyIp = proxyHost;
            }

            var baseAddress = new Uri($"http://127.0.0.1:{port}/");
            var proxyAddress = new Uri($"http://{proxyHost}:{port}/");
            var proxyIpAddress = new Uri($"http://{proxyIp}:{port}/");

            return new TestWebHost(app, baseAddress, proxyAddress, proxyIpAddress);
        }

        public ValueTask DisposeAsync()
        {
            return _application.DisposeAsync();
        }

        private static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
#endif
