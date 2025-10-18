using System.Collections.Generic;
using System.Text;
using Xunit;

namespace xNet.Tests.Unit
{
    public class HttpHelperTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("hello", "hello")]
        [InlineData("hello world", "hello+world")]
        [InlineData("café", "caf%C3%A9")]
        [InlineData("a+b", "a%2Bb")]
        public void UrlEncode_EncodesUnsafeCharacters(string input, string expected)
        {
            string encoded = Http.UrlEncode(input, Encoding.UTF8);

            Assert.Equal(expected, encoded);
        }

        [Fact]
        public void ToQueryString_SkipsEmptyKeysAndEscapesValues()
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new("alpha", "1 2"),
                new("", "should-skip"),
                new("bravo", "café")
            };

            string query = Http.ToQueryString(parameters, dontEscape: false);

            Assert.Equal("alpha=1%202&bravo=caf%C3%A9", query);
        }

        [Fact]
        public void ToPostQueryString_UsesUrlEncodeWithCustomEncoding()
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new("greeting", "привет"),
            };

            string query = Http.ToPostQueryString(parameters, dontEscape: false, Encoding.UTF8);

            Assert.Equal("greeting=%D0%BF%D1%80%D0%B8%D0%B2%D0%B5%D1%82", query);
        }
    }
}
