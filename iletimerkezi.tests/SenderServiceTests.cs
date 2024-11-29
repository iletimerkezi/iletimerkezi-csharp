using System;
using System.Threading.Tasks;
using IletiMerkezi.Services;
using IletiMerkezi.Tests.Mocks;
using Xunit;
using System.Linq;

namespace IletiMerkezi.Tests.Services
{
    public class SenderServiceTests
    {
        private const string API_KEY = "test-key";
        private const string API_HASH = "test-hash";

        [Fact]
        public async Task ListAsync_WhenSuccessful_ReturnsApprovedSenders()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 200,
                        ""message"": ""İşlem başarılı""
                    },
                    ""senders"": {
                        ""sender"": [
                            ""SENDER1"",
                            ""SENDER2"",
                            ""SENDER3""
                        ]
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse);
            var senderService = new SenderService(mockHttpClient, API_KEY, API_HASH);

            // Act
            var result = await senderService.ListAsync();

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("İşlem başarılı", result.Message);
            Assert.Equal(3, result.Response.Data.Senders.Sender.Count);
            Assert.Contains("SENDER1", result.Response.Data.Senders.Sender);
            Assert.Contains("SENDER2", result.Response.Data.Senders.Sender);
            Assert.Contains("SENDER3", result.Response.Data.Senders.Sender);
        }

        [Fact]
        public async Task ListAsync_WhenError_ReturnsErrorResponse()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 401,
                        ""message"": ""Üyelik bilgileri hatalı""
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse, 401);
            var senderService = new SenderService(mockHttpClient, API_KEY, API_HASH);

            // Act
            var result = await senderService.ListAsync();

            // Assert
            Assert.False(result.Ok);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Üyelik bilgileri hatalı", result.Message);
        }

        [Fact]
        public async Task ListAsync_VerifyRequestPayload()
        {
            // Arrange
            var mockHttpClient = new MockHttpClient("{}");
            var senderService = new SenderService(mockHttpClient, API_KEY, API_HASH);

            // Act
            await senderService.ListAsync();

            // Assert
            var lastPayload = mockHttpClient.GetLastPayload();
            var expectedJson = @"{
                ""request"": {
                    ""authentication"": {
                        ""key"": """ + API_KEY + @""",
                        ""hash"": """ + API_HASH + @"""
                    }
                }
            }";

            Assert.Equal(
                System.Text.Json.JsonSerializer.Serialize(
                    System.Text.Json.JsonSerializer.Deserialize<dynamic>(expectedJson)
                ),
                System.Text.Json.JsonSerializer.Serialize(
                    System.Text.Json.JsonSerializer.Deserialize<dynamic>(lastPayload)
                )
            );
        }
    }
}