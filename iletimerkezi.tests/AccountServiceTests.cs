using System;
using System.Threading.Tasks;
using IletiMerkezi.Services;
using IletiMerkezi.Tests.Mocks;
using Xunit;
using System.Text.Json;

namespace IletiMerkezi.Tests.Services
{
    public class AccountServiceTests
    {
        private const string API_KEY = "test-key";
        private const string API_HASH = "test-hash";

        [Fact]
        public async Task GetBalanceAsync_WhenSuccessful_ReturnsCorrectBalance()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 200,
                        ""message"": ""İşlem başarılı""
                    },
                    ""balance"": {
                        ""amount"": 100.50,
                        ""sms"": 1000
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse);
            var accountService = new AccountService(mockHttpClient, API_KEY, API_HASH);

            // Act
            var result = await accountService.GetBalanceAsync();

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("İşlem başarılı", result.Message);
            Assert.Equal(100.50m, result.Amount);
            Assert.Equal(1000, result.Credits);
        }

        [Fact]
        public async Task GetBalanceAsync_WhenError_ReturnsErrorResponse()
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
            var accountService = new AccountService(mockHttpClient, API_KEY, API_HASH);

            // Act
            var result = await accountService.GetBalanceAsync();

            // Assert
            Assert.False(result.Ok);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Üyelik bilgileri hatalı", result.Message);
            Assert.Equal(5m, result.Amount); // Default value
            Assert.Equal(5, result.Credits); // Default value
        }

        [Fact]
        public async Task GetBalanceAsync_VerifyRequestPayload()
        {
            // Arrange
            var mockHttpClient = new MockHttpClient("{}");
            var accountService = new AccountService(mockHttpClient, API_KEY, API_HASH);

            // Act
            await accountService.GetBalanceAsync();

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