using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Moq;
using Moq.Protected;
using Xunit;

namespace PartyTube.ServiceTests
{
    // todo в отдельную библиотеку
    public class HttpClientMock
    {
        private const string SendAsyncMethodName = "SendAsync";
        public readonly Mock<HttpClient> HttpClient;
        public readonly Mock<HttpContent> HttpContent;
        public readonly Mock<HttpMessageHandler> HttpMessageHandler;
        public readonly Mock<HttpResponseMessage> HttpResponseMessage;

        public HttpClientMock()
        {
            HttpMessageHandler = new Mock<HttpMessageHandler>();
            HttpResponseMessage = new Mock<HttpResponseMessage>();
            HttpContent = new Mock<HttpContent>();
            HttpResponseMessage.Object.Content = HttpContent.Object;
            HttpMessageHandler.Protected()
                              .Setup<Task<HttpResponseMessage>>(SendAsyncMethodName,
                                                                ItExpr.IsAny<HttpRequestMessage>(),
                                                                ItExpr.IsAny<CancellationToken>())
                              .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                                                HttpResponseMessage.Object)
                              .Verifiable();

            HttpClient = new Mock<HttpClient>(HttpMessageHandler.Object);
        }

        public void SetResponseMessage(string message)
        {
            HttpResponseMessage.Object.Content = new StringContent(message);
        }

        public void VerifySendAsync(Times times, params object[] args)
        {
            HttpMessageHandler.Protected().Verify(SendAsyncMethodName, times, args);
        }

        public void VerifySendAsyncUri(Times times, Uri expected)
        {
            HttpMessageHandler.Protected()
                              .Verify(SendAsyncMethodName,
                                      times,
                                      ItExpr.Is<HttpRequestMessage>(
                                          message => IsUriEquals(expected, message.RequestUri)),
                                      ItExpr.IsAny<CancellationToken>());
        }

        public void VerifySendAsyncUri(Times times, string expected)
        {
            VerifySendAsyncUri(times, new Uri(expected));
        }

        public void VerifySendAsync(Times times)
        {
            HttpMessageHandler.Protected()
                              .Verify(SendAsyncMethodName,
                                      times,
                                      ItExpr.IsAny<HttpRequestMessage>(),
                                      ItExpr.IsAny<CancellationToken>());
        }

        // todo в отдельную библиотеку
        private bool IsUriEquals(Uri expectedUri, Uri actualUri)
        {
            if (Uri.Compare(expectedUri,
                            actualUri,
                            UriComponents.HostAndPort
                          | UriComponents.Path,
                            UriFormat.SafeUnescaped,
                            StringComparison.OrdinalIgnoreCase)
             != 0) return false;

            if (string.IsNullOrWhiteSpace(expectedUri.Query) && string.IsNullOrWhiteSpace(actualUri.Query))
                return true;

            var expectedItems = GetQueryKeyValue(expectedUri);
            var actualItems = GetQueryKeyValue(actualUri);

            Assert.Equal(expectedItems, actualItems);

            return true;
        }

        private IEnumerable<(string key, string value)> GetQueryKeyValue(Uri uri)
        {
            var queryCollection = HttpUtility.ParseQueryString(uri.Query);
            var result = queryCollection.Cast<string>()
                                        .Select(s => (key: s, value: queryCollection[s]))
                                        .OrderBy(o => o.key);
            return result;
        }
    }
}