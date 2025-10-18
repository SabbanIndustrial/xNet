using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace xNet.Tests
{
    public class HttpRequestOptionsTests
    {
        [Fact]
        public void OptionsAreAppliedToRequest()
        {
            var options = HttpRequestOptions.CreateDefault();
            options.Timeouts.ConnectTimeout = TimeSpan.FromSeconds(10);
            options.Timeouts.ReadWriteTimeout = TimeSpan.FromSeconds(20);
            options.Timeouts.KeepAliveTimeout = TimeSpan.FromSeconds(5);
            options.Retry.IsEnabled = true;
            options.Retry.MaxRetries = 5;
            options.Retry.BaseDelay = TimeSpan.FromMilliseconds(250);
            options.DefaultHeaders["User-Agent"] = "TestAgent/1.0";
            options.DefaultHeaders["X-Correlation-Id"] = "abc";

            var request = new HttpRequest(options);

            Assert.Equal(10_000, request.ConnectTimeout);
            Assert.Equal(20_000, request.ReadWriteTimeout);
            Assert.Equal(5_000, request.KeepAliveTimeout);
            Assert.True(request.Reconnect);
            Assert.Equal(5, request.ReconnectLimit);
            Assert.Equal(250, request.ReconnectDelay);
            Assert.Equal("TestAgent/1.0", request["User-Agent"]);
            Assert.Equal("abc", request["X-Correlation-Id"]);
        }

        [Fact]
        public void CloneProducesDistinctInstances()
        {
            var options = HttpRequestOptions.CreateDefault();
            options.DefaultHeaders["X-Test"] = "original";

            var clone = options.Clone();
            clone.DefaultHeaders["X-Test"] = "clone";

            Assert.Equal("original", options.DefaultHeaders["X-Test"]);
            Assert.Equal("clone", clone.DefaultHeaders["X-Test"]);
            Assert.NotSame(options.Timeouts, clone.Timeouts);
            Assert.NotSame(options.Retry, clone.Retry);
        }
    }

    public class HttpRequestProgressTests
    {
        [Fact]
        public void ProgressReportersReceiveTotals()
        {
            var request = new HttpRequest();
            var uploadProgress = new TestProgress();
            var downloadProgress = new TestProgress();

            long? uploadTotal = null;
            long? downloadTotal = null;

            request.UploadProgress = uploadProgress;
            request.DownloadProgress = downloadProgress;

            request.UploadProgressChanged += (_, args) => uploadTotal = args.TotalBytesToSend;
            request.DownloadProgressChanged += (_, args) => downloadTotal = args.TotalBytesToReceive;

            var totalBytesSentField = typeof(HttpRequest).GetField("_totalBytesSent", BindingFlags.Instance | BindingFlags.NonPublic);
            totalBytesSentField!.SetValue(request, 100L);

            var reportBytesSent = typeof(HttpRequest).GetMethod("ReportBytesSent", BindingFlags.Instance | BindingFlags.NonPublic);
            reportBytesSent!.Invoke(request, new object[] { 25 });

            var canReportField = typeof(HttpRequest).GetField("_canReportBytesReceived", BindingFlags.Instance | BindingFlags.NonPublic);
            canReportField!.SetValue(request, true);

            var totalBytesReceivedField = typeof(HttpRequest).GetField("_totalBytesReceived", BindingFlags.Instance | BindingFlags.NonPublic);
            totalBytesReceivedField!.SetValue(request, 200L);

            var reportBytesReceived = typeof(HttpRequest).GetMethod("ReportBytesReceived", BindingFlags.Instance | BindingFlags.NonPublic);
            reportBytesReceived!.Invoke(request, new object[] { 50 });

            Assert.Equal(new long[] { 25 }, uploadProgress.Values);
            Assert.Equal(100L, uploadTotal);
            Assert.Equal(new long[] { 50 }, downloadProgress.Values);
            Assert.Equal(200L, downloadTotal);
        }

        private sealed class TestProgress : IProgress<long>
        {
            public IList<long> Values { get; } = new List<long>();

            public void Report(long value)
            {
                Values.Add(value);
            }
        }
    }
}
