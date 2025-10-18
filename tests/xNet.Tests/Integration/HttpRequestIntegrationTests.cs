#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HttpMethod = xNet.HttpMethod;
using HttpResponse = xNet.HttpResponse;
using Xunit;

namespace xNet.Tests.Integration
{
    public class HttpRequestIntegrationTests
    {
        [Fact]
        public async Task GetRequest_ReturnsExpectedBody()
        {
            await using var server = await TestWebHost.StartAsync(app =>
            {
                app.MapGet("/text", () => Results.Text("Hello from xNet", "text/plain"));
            });

            using var request = new HttpRequest();
            HttpResponse response = request.Get(new Uri(server.BaseAddress, "text"));
            string body = response.ToString();

            Assert.Equal("Hello from xNet", body);
        }

        [Fact]
        public async Task CompressionResponse_IsAutomaticallyDecompressed()
        {
            await using var server = await TestWebHost.StartAsync(app =>
            {
                app.MapGet("/compressed", async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "text/plain";
                    context.Response.Headers["Content-Encoding"] = "gzip";

                    await using var gzip = new GZipStream(context.Response.Body, CompressionLevel.Optimal, leaveOpen: true);
                    await using var writer = new StreamWriter(gzip, Encoding.UTF8, leaveOpen: true);
                    await writer.WriteAsync("Compressed payload");
                });
            });

            using var request = new HttpRequest();
            request.EnableEncodingContent = true;

            HttpResponse response = request.Get(new Uri(server.BaseAddress, "compressed"));
            string body = response.ToString();

            Assert.Equal("Compressed payload", body);
        }

        [Fact]
        public async Task Redirects_AreFollowedByDefault()
        {
            await using var server = await TestWebHost.StartAsync(app =>
            {
                app.MapGet("/redirect", context =>
                {
                    context.Response.StatusCode = StatusCodes.Status302Found;
                    context.Response.Headers["Location"] = "/final";
                    return Task.CompletedTask;
                });

                app.MapGet("/final", () => Results.Text("followed", "text/plain"));
            });

            using var request = new HttpRequest();
            HttpResponse response = request.Get(new Uri(server.BaseAddress, "redirect"));
            string body = response.ToString();

            Assert.Equal("followed", body);
        }

        [Fact]
        public async Task ProgressCallbacks_ReportTransferredBytes()
        {
            await using var server = await TestWebHost.StartAsync(app =>
            {
                app.MapPost("/echo", async context =>
                {
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
                    string payload = await reader.ReadToEndAsync();

                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "text/plain";
                    context.Response.ContentLength = Encoding.UTF8.GetByteCount(payload);
                    await context.Response.WriteAsync(payload);
                });
            });

            using var request = new HttpRequest();
            var uploadProgress = new List<int>();
            var downloadProgress = new List<int>();
            request.UploadProgressChanged += (_, args) => uploadProgress.Add(args.BytesSent);
            request.DownloadProgressChanged += (_, args) => downloadProgress.Add(args.BytesReceived);

            string message = new string('a', 2048);
            byte[] payload = Encoding.UTF8.GetBytes(message);

            HttpResponse response;
            using (var content = new BytesContent(payload))
            {
                response = request.Raw(HttpMethod.POST, new Uri(server.BaseAddress, "echo"), content);
            }

            string echoed = response.ToString();

            Assert.Equal(message, echoed);
            Assert.NotEmpty(uploadProgress);
            Assert.NotEmpty(downloadProgress);
            Assert.Equal(payload.Length, uploadProgress[^1]);
            Assert.Equal(payload.Length, downloadProgress[^1]);
        }
    }
}
#endif
