using System;
using Xunit;
using IletiMerkezi.Services;
using System.Text.Json;

namespace IletiMerkezi.Tests.Services
{
    public class WebhookServiceTests
    {
        private readonly WebhookService _webhookService;

        public WebhookServiceTests()
        {
            _webhookService = new WebhookService();
        }

        [Fact]
        public void HandleJson_ValidWebhook_ParsesCorrectly()
        {
            // Arrange
            var webhookData = @"{
                ""report"": {
                    ""id"": 1599558518,
                    ""packet_id"": 104525848,
                    ""status"": ""accepted"",
                    ""to"": ""+90505702xxxx"",
                    ""body"": ""Webhook test mesaji""
                }
            }";

            // Act
            var result = _webhookService.Handle(webhookData);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Report);
            Assert.Equal(1599558518, result.Id);
            Assert.Equal(104525848, result.PacketId);
            Assert.Equal("accepted", result.Status);
            Assert.Equal("+90505702xxxx", result.To);
            Assert.Equal("Webhook test mesaji", result.Body);
        }

        [Fact]
        public void Handle_FromHttpRequest_ParsesCorrectly()
        {
            // Arrange
            var webhookData = @"{
                ""report"": {
                    ""id"": 1599558518,
                    ""packet_id"": 104525848,
                    ""status"": ""accepted"",
                    ""to"": ""+90505702xxxx"",
                    ""body"": ""Webhook test mesaji""
                }
            }";

            // Act
            var result = _webhookService.Handle(webhookData);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Report);
            Assert.Equal(1599558518, result.Id);
            Assert.Equal(104525848, result.PacketId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void HandleJson_InvalidWebhookData_ThrowsArgumentException(string invalidData)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _webhookService.Handle(invalidData));
        }

        [Fact]
        public void HandleJson_InvalidJsonFormat_ThrowsJsonException()
        {
            // Arrange
            var invalidJson = "{invalid-json}";

            // Act & Assert
            Assert.Throws<JsonException>(() => _webhookService.Handle(invalidJson));
        }
    }
}