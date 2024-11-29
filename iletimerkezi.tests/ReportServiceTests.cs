using System;
using System.Threading.Tasks;
using IletiMerkezi.Http;
using IletiMerkezi.Models;
using IletiMerkezi.Services;
using Moq;
using Xunit;

namespace IletiMerkezi.Tests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<IHttpClient> _mockHttpClient;
        private readonly ReportService _reportService;
        private const string API_KEY = "test-api-key";
        private const string API_HASH = "test-api-hash";

        public ReportServiceTests()
        {
            _mockHttpClient = new Mock<IHttpClient>();
            _reportService = new ReportService(_mockHttpClient.Object, API_KEY, API_HASH);
        }

        [Fact]
        public async Task GetDeliveryReport_ShouldReturnDeliveryReport_WhenValidRequestMade()
        {
            // Arrange
            var id = 12345;
            var expectedResponse = new HttpResponse
            {
                Body = @"{
                    ""response"": {
                        ""status"": {
                            ""code"": 200,
                            ""message"": ""OK""
                        },
                        ""order"": {
                            ""id"": 12345,
                            ""status"": 114,
                            ""total"": 1,
                            ""delivered"": 1,
                            ""undelivered"": 0,
                            ""waiting"": 0,
                            ""submitAt"": ""2024-03-20 10:00:00"",
                            ""sendAt"": ""2024-03-20 10:01:00"",
                            ""sender"": ""Test Sender"",
                            ""price"": 0.5,
                            ""message"": [
                                {
                                    ""number"": ""+905321234567"",
                                    ""status"": 110
                                }
                            ]
                        }
                    }
                }"
            };

            _mockHttpClient
                .Setup(x => x.PostAsync("get-report/json", It.IsAny<object>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _reportService.GetAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("OK", result.Message);
            Assert.Equal(id, result.Id);
            Assert.Equal("COMPLETED", result.StatusText);
            Assert.Equal(114, result.Status);
            Assert.Equal(1, result.Total);
            Assert.Equal(1, result.Delivered);
            Assert.Equal(0, result.Undelivered);
            Assert.Equal(0, result.Waiting);
            Assert.Equal("2024-03-20 10:00:00", result.SubmitAt);
            Assert.Equal("2024-03-20 10:01:00", result.SendAt);
            Assert.Equal("Test Sender", result.Sender);
            Assert.Single(result.Messages);
            Assert.Equal("+905321234567", result.Messages[0].Number);
            Assert.Equal("WAITING", result.Messages[0].StatusText);

            _mockHttpClient.Verify(x => x.PostAsync("get-report/json", It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GetDeliveryReport_ShouldThrowException_WhenHttpClientFails()
        {
            // Arrange
            var id = 12345;
            _mockHttpClient
                .Setup(x => x.PostAsync("get-report/json", It.IsAny<object>()))
                .ThrowsAsync(new Exception("HTTP request failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _reportService.GetAsync(id));
        }
    }
} 