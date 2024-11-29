using System;
using System.Threading.Tasks;
using IletiMerkezi.Services;
using IletiMerkezi.Tests.Mocks;
using Xunit;
using System.Text.Json;

namespace IletiMerkezi.Tests.Services
{
    public class SummaryServiceTests
    {
        private const string API_KEY = "test-key";
        private const string API_HASH = "test-hash";

        [Fact]
        public async Task ListAsync_WhenSuccessful_ReturnsOrdersList()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 200,
                        ""message"": ""İşlem başarılı""
                    },
                    ""count"": 2,
                    ""orders"": [
                        {
                            ""id"": 89,
                            ""status"": 113,
                            ""total"": 1,
                            ""delivered"": 0,
                            ""undelivered"": 0,
                            ""waiting"": 1,
                            ""submitAt"": ""2024-11-12 02:04:08"",
                            ""sendAt"": ""2024-11-12 02:04:08"",
                            ""sender"": ""eMarka""
                        },
                        {
                            ""id"": 98,
                            ""status"": 113,
                            ""total"": 1,
                            ""delivered"": 0,
                            ""undelivered"": 0,
                            ""waiting"": 1,
                            ""submitAt"": ""2024-11-13 19:04:08"",
                            ""sendAt"": ""2024-11-13 19:04:08"",
                            ""sender"": ""eMarka""
                        }
                    ]
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse);
            var summaryService = new SummaryService(mockHttpClient, API_KEY, API_HASH);
            var startDate = "2024-11-11";
            var endDate = "2024-11-20";

            // Act
            var result = await summaryService.ListAsync(startDate, endDate);

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("İşlem başarılı", result.Message);
            Assert.Equal(2, result.Count);
            Assert.Equal(2, result.Orders.Count);
            
            var firstOrder = result.Orders[0];
            Assert.Equal(89, firstOrder.Id);
            Assert.Equal(113, firstOrder.Status);
            Assert.Equal(1, firstOrder.Total);
            Assert.Equal(0, firstOrder.Delivered);
            Assert.Equal(0, firstOrder.Undelivered);
            Assert.Equal(1, firstOrder.Waiting);
            Assert.Equal("2024-11-12 02:04:08", firstOrder.SubmitAt);
            Assert.Equal("2024-11-12 02:04:08", firstOrder.SendAt);
            Assert.Equal("eMarka", firstOrder.Sender);
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
            var summaryService = new SummaryService(mockHttpClient, API_KEY, API_HASH);
            var startDate = "2024-11-11";
            var endDate = "2024-11-20";

            // Act
            var result = await summaryService.ListAsync(startDate, endDate);

            // Assert
            Assert.False(result.Ok);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Üyelik bilgileri hatalı", result.Message);
            Assert.Empty(result.Orders);
        }

        [Fact]
        public async Task ListAsync_VerifyRequestPayload()
        {
            // Arrange
            var mockHttpClient = new MockHttpClient("{}");
            var summaryService = new SummaryService(mockHttpClient, API_KEY, API_HASH);
            var startDate = "2024-11-11";
            var endDate = "2024-11-20";
            var page = 1;

            // Act
            await summaryService.ListAsync(startDate, endDate, page);

            // Assert
            var lastPayload = mockHttpClient.GetLastPayload();
            var expectedJson = @"{
                ""request"": {
                    ""authentication"": {
                        ""key"": """ + API_KEY + @""",
                        ""hash"": """ + API_HASH + @"""
                    },
                    ""filter"": {
                        ""start"": """ + startDate + @""",
                        ""end"": """ + endDate + @""",
                        ""page"": " + page + @"
                    }
                }
            }";

            Assert.Equal(
                JsonSerializer.Serialize(
                    JsonSerializer.Deserialize<dynamic>(expectedJson)
                ),
                JsonSerializer.Serialize(
                    JsonSerializer.Deserialize<dynamic>(lastPayload)
                )
            );
        }
    }
}