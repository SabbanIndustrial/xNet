using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace xNet.Tests.Unit
{
    public class HttpRequestHeaderTests
    {
        [Fact]
        public void MergeHeaders_PrioritizesSourceValues()
        {
            var request = new HttpRequest();
            var destination = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["X-Test"] = "permanent",
                ["X-Another"] = "value"
            };

            var source = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["X-Test"] = "temporary",
                ["X-New"] = "added"
            };

            InvokeMergeHeaders(request, destination, source);

            Assert.Equal("temporary", destination["X-Test"]);
            Assert.Equal("value", destination["X-Another"]);
            Assert.Equal("added", destination["X-New"]);
        }

        private static void InvokeMergeHeaders(HttpRequest request, Dictionary<string, string> destination, Dictionary<string, string> source)
        {
            MethodInfo? mergeHeaders = typeof(HttpRequest).GetMethod("MergeHeaders", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(mergeHeaders);

            mergeHeaders!.Invoke(request, new object[] { destination, source });
        }
    }
}
