using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using IletiMerkezi.Services;
using IletiMerkezi.Tests.Mocks;
using Xunit;
using System.Text.Json;

namespace IletiMerkezi.Tests.Services
{
    public class SmsServiceTests
    {
        private const string API_KEY = "test-key";
        private const string API_HASH = "test-hash";
        private const string DEFAULT_SENDER = "TESTSENDER";

        [Fact]
        public async Task SendAsync_SingleRecipient_ReturnsSuccessResponse()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 200,
                        ""message"": ""İşlem başarılı""
                    },
                    ""order"": {
                        ""id"": 12345
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse);
            var smsService = new SmsService(mockHttpClient, API_KEY, API_HASH, DEFAULT_SENDER);

            // Act
            var result = await smsService.SendAsync("5551234567", "Test message");

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("İşlem başarılı", result.Message);
            Assert.Equal(12345, result.Response.Data.Order.Id);
        }

        [Fact]
        public async Task SendAsync_MultipleRecipients_ReturnsSuccessResponse()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 200,
                        ""message"": ""İşlem başarılı""
                    },
                    ""order"": {
                        ""id"": 12345
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse);
            var smsService = new SmsService(mockHttpClient, API_KEY, API_HASH, DEFAULT_SENDER);
            var recipients = new List<string> { "5551234567", "5551234568", "5551234569" };

            // Act
            var result = await smsService.SendAsync(recipients, "Test message");

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(12345, result.Response.Data.Order.Id);
        }

        [Fact]
        public async Task SendAsync_WithCustomConfiguration_VerifyRequestPayload()
        {
            // Arrange
            var mockHttpClient = new MockHttpClient("{}");
            var smsService = new SmsService(mockHttpClient, API_KEY, API_HASH, DEFAULT_SENDER);
            var sendDateTime = "2024-03-25 10:00:00";

            // Act
            await smsService
                .Schedule(sendDateTime)
                .EnableIysConsent()
                .SetIysList("TICARI")
                .SendAsync("5551234567", "Test message");

            // Assert
            var lastPayload = mockHttpClient.GetLastPayload();
            var expectedJson = @"{
                ""request"": {
                    ""authentication"": {
                        ""key"": """ + API_KEY + @""",
                        ""hash"": """ + API_HASH + @"""
                    },
                    ""order"": {
                        ""sender"": """ + DEFAULT_SENDER + @""",
                        ""sendDateTime"": """ + sendDateTime + @""",
                        ""iys"": 1,
                        ""iysList"": ""TICARI"",
                        ""message"": {
                            ""text"": ""Test message"",
                            ""receipents"": {
                                ""number"": [""5551234567""]
                            }
                        }
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

        [Fact]
        public async Task CancelAsync_ReturnsSuccessResponse()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 200,
                        ""message"": ""İşlem başarılı""
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse);
            var smsService = new SmsService(mockHttpClient, API_KEY, API_HASH, DEFAULT_SENDER);
            var orderId = 12345;

            // Act
            var result = await smsService.CancelAsync(orderId);

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("İşlem başarılı", result.Message);
        }
    }
}