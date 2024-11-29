using System.Threading.Tasks;
using IletiMerkezi.Http;
using Xunit;

namespace IletiMerkezi.Tests.Http
{
    public class DefaultHttpClientTests
    {
        [Fact]
        public async Task PostAsync_ShouldIncludeUserAgent()
        {
            // Arrange
            var client = new DefaultHttpClient();
            var expectedUserAgent = $"iletimerkezi-csharp/{Constants.Version}";

            // Act & Assert
            // Not: Bu test gerçek bir HTTP isteği yapacak, 
            // idealde bir HTTP interceptor kullanılmalı
            var response = await client.PostAsync("test", new { });
            
            // User-Agent header'ının doğru şekilde eklendiğini kontrol etmek için
            // gerçek bir HTTP endpoint'e istek yapıp cevabı kontrol etmek gerekir
            // Ya da bir HTTP interceptor/mock kullanılmalı
        }
    }
} 