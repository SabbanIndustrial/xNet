#if NET6_0_OR_GREATER
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;
using HttpResponse = xNet.HttpResponse;

namespace xNet.Tests.Proxies
{
    public class ProxyClientEndToEndTests
    {
        [Fact]
        public async Task HttpProxyClient_ForwardRequest()
        {
            var settings = ProxyEnvironment.Http;
            Skip.If(settings == null, "HTTP proxy configuration is not available.");

            await using var server = await StartProxyAwareServer();
            using var request = new HttpRequest();
            request.Proxy = new HttpProxyClient(settings!.Value.host, settings.Value.port);

            HttpResponse response = request.Get(new Uri(server.ProxyAccessibleBaseAddress, "proxy"));
            string body = response.ToString();

            Assert.Equal("proxied", body);
        }

        [Fact]
        public async Task Socks4ProxyClient_ForwardRequest()
        {
            var settings = ProxyEnvironment.Socks4;
            Skip.If(settings == null, "SOCKS4 proxy configuration is not available.");

            await using var server = await StartProxyAwareServer();
            using var request = new HttpRequest();
            request.Proxy = new Socks4ProxyClient(settings!.Value.host, settings.Value.port);

            HttpResponse response = request.Get(new Uri(server.ProxyAccessibleIpBaseAddress, "proxy"));
            string body = response.ToString();

            Assert.Equal("proxied", body);
        }

        [Fact]
        public async Task Socks4aProxyClient_ForwardRequest()
        {
            var settings = ProxyEnvironment.Socks4A;
            Skip.If(settings == null, "SOCKS4a proxy configuration is not available.");

            await using var server = await StartProxyAwareServer();
            using var request = new HttpRequest();
            request.Proxy = new Socks4aProxyClient(settings!.Value.host, settings.Value.port);

            HttpResponse response = request.Get(new Uri(server.ProxyAccessibleBaseAddress, "proxy"));
            string body = response.ToString();

            Assert.Equal("proxied", body);
        }

        [Fact]
        public async Task Socks5ProxyClient_ForwardRequest()
        {
            var settings = ProxyEnvironment.Socks5;
            Skip.If(settings == null, "SOCKS5 proxy configuration is not available.");

            await using var server = await StartProxyAwareServer();
            using var request = new HttpRequest();
            request.Proxy = new Socks5ProxyClient(settings!.Value.host, settings.Value.port);

            HttpResponse response = request.Get(new Uri(server.ProxyAccessibleBaseAddress, "proxy"));
            string body = response.ToString();

            Assert.Equal("proxied", body);
        }

        private static Task<TestWebHost> StartProxyAwareServer()
        {
            return TestWebHost.StartAsync(app =>
            {
                app.MapGet("/proxy", () => Results.Text("proxied", "text/plain"));
            });
        }
    }

    internal static class ProxyEnvironment
    {
        public static (string host, int port)? Http => Read("XNET_HTTP_PROXY_HOST", "XNET_HTTP_PROXY_PORT");

        public static (string host, int port)? Socks4 => Read("XNET_SOCKS4_PROXY_HOST", "XNET_SOCKS4_PROXY_PORT");

        public static (string host, int port)? Socks4A => Read("XNET_SOCKS4A_PROXY_HOST", "XNET_SOCKS4A_PROXY_PORT");

        public static (string host, int port)? Socks5 => Read("XNET_SOCKS5_PROXY_HOST", "XNET_SOCKS5_PROXY_PORT");

        private static (string host, int port)? Read(string hostVar, string portVar)
        {
            string? host = Environment.GetEnvironmentVariable(hostVar);
            string? portValue = Environment.GetEnvironmentVariable(portVar);

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(portValue) || !int.TryParse(portValue, out int port))
            {
                return null;
            }

            return (host, port);
        }
    }
}
#endif
