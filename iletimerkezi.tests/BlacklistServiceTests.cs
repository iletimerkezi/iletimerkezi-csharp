using System;
using System.Threading.Tasks;
using IletiMerkezi.Services;
using IletiMerkezi.Tests.Mocks;
using Xunit;

namespace IletiMerkezi.Tests.Services
{
    public class BlacklistServiceTests
    {
        private const string API_KEY = "test-key";
        private const string API_HASH = "test-hash";

        [Fact]
        public async Task CreateAsync_WhenSuccessful_ReturnsSuccessResponse()
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
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var number = "905321234567";

            // Act
            var result = await blacklistService.CreateAsync(number);

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("İşlem başarılı", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WhenError_ReturnsErrorResponse()
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
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var number = "905321234567";

            // Act
            var result = await blacklistService.CreateAsync(number);

            // Assert
            Assert.False(result.Ok);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Üyelik bilgileri hatalı", result.Message);
        }

        [Fact]
        public async Task CreateAsync_VerifyRequestPayload()
        {
            // Arrange
            var mockHttpClient = new MockHttpClient("{}");
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var number = "905321234567";

            // Act
            await blacklistService.CreateAsync(number);

            // Assert
            var lastPayload = mockHttpClient.GetLastPayload();
            var expectedJson = @"{
                ""request"": {
                    ""authentication"": {
                        ""key"": """ + API_KEY + @""",
                        ""hash"": """ + API_HASH + @"""
                    },
                    ""blacklist"": {
                        ""number"": """ + number + @"""
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

        [Fact]
        public async Task DeleteAsync_WhenSuccessful_ReturnsSuccessResponse()
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
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var number = "905321234567";

            // Act
            var result = await blacklistService.DeleteAsync(number);

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("İşlem başarılı", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WhenError_ReturnsErrorResponse()
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
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var number = "905321234567";

            // Act
            var result = await blacklistService.DeleteAsync(number);

            // Assert
            Assert.False(result.Ok);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Üyelik bilgileri hatalı", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_VerifyRequestPayload()
        {
            // Arrange
            var mockHttpClient = new MockHttpClient("{}");
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var number = "905321234567";

            // Act
            await blacklistService.DeleteAsync(number);

            // Assert
            var lastPayload = mockHttpClient.GetLastPayload();
            var expectedJson = @"{
                ""request"": {
                    ""authentication"": {
                        ""key"": """ + API_KEY + @""",
                        ""hash"": """ + API_HASH + @"""
                    },
                    ""blacklist"": {
                        ""number"": """ + number + @"""
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

        [Fact]
        public async Task ListAsync_WhenSuccessful_ReturnsBlacklistNumbers()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 200,
                        ""message"": ""İşlem başarılı""
                    },
                    ""blacklist"": {
                        ""count"": 2,
                        ""number"": [
                            ""+905057028998"",
                            ""+905057028912""
                        ]
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse);
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var startDate = new DateTime(2024, 9, 16, 0, 16, 3);
            var endDate = new DateTime(2024, 9, 16, 23, 59, 3);

            // Act
            var result = await blacklistService.ListAsync(startDate, endDate, 1, 10);

            // Assert
            Assert.True(result.Ok);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("İşlem başarılı", result.Message);
            Assert.Equal(2, result.Count);
            Assert.Contains("+905057028998", result.Numbers);
            Assert.Contains("+905057028912", result.Numbers);
        }

        [Fact]
        public async Task ListAsync_WhenDateFormatError_ReturnsValidationError()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 422,
                        ""message"": ""İstekte gönderilen bazı değerler doğrulanamadı""
                    },
                    ""errors"": {
                        ""start"": ""start Y-m-d H:i:s biçimi ile eşleşmiyor."",
                        ""end"": ""end Y-m-d H:i:s biçimi ile eşleşmiyor.""
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse, 422);
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var startDate = new DateTime(2024, 9, 16, 0, 16, 3);
            var endDate = new DateTime(2024, 9, 16, 23, 59, 3);

            // Act
            var result = await blacklistService.ListAsync(startDate, endDate, 1, 10);

            // Assert
            Assert.False(result.Ok);
            Assert.Equal(422, result.StatusCode);
            Assert.Equal("İstekte gönderilen bazı değerler doğrulanamadı", result.Message);
        }

        [Fact]
        public async Task ListAsync_WhenGeneralError_ReturnsErrorResponse()
        {
            // Arrange
            var mockResponse = @"{
                ""response"": {
                    ""status"": {
                        ""code"": 422,
                        ""message"": ""İstekte gönderilen bazı değerler doğrulanamadı""
                    }
                }
            }";

            var mockHttpClient = new MockHttpClient(mockResponse, 422);
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var startDate = new DateTime(2024, 9, 16, 0, 16, 3);
            var endDate = new DateTime(2024, 9, 16, 23, 59, 3);

            // Act
            var result = await blacklistService.ListAsync(startDate, endDate, 1, 10);

            // Assert
            Assert.False(result.Ok);
            Assert.Equal(422, result.StatusCode);
            Assert.Equal("İstekte gönderilen bazı değerler doğrulanamadı", result.Message);
        }

        [Fact]
        public async Task ListAsync_VerifyRequestPayload()
        {
            // Arrange
            var mockHttpClient = new MockHttpClient("{}");
            var blacklistService = new BlacklistService(mockHttpClient, API_KEY, API_HASH);
            var startDate = new DateTime(2024, 9, 16, 0, 16, 3);
            var endDate = new DateTime(2024, 9, 16, 23, 59, 3);

            // Act
            await blacklistService.ListAsync(startDate, endDate, 1, 10);

            // Assert
            var lastPayload = mockHttpClient.GetLastPayload();

            var expectedJson = @"{
                ""request"": {
                    ""authentication"": {
                        ""key"": """ + API_KEY + @""",
                        ""hash"": """ + API_HASH + @"""
                    },
                    ""blacklist"": {
                        ""filter"": {
                            ""start"": ""2024-09-16 00:16:03"",
                            ""end"": ""2024-09-16 23:59:03""
                        },
                        ""page"": 1,
                        ""rowCount"": 10
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