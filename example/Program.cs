using System;
using System.Threading.Tasks;
using IletiMerkezi;
using IletiMerkezi.Http;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new IletiMerkeziClient(
            "api-key",
            "api-secret",
            "your-sender"
        );

        var balanceService = client.Senders();
        var balance = await balanceService.ListAsync();

        Console.WriteLine(client.Debug());

        if (balance.Ok)
        {
            Console.WriteLine($"Response: {balance.Response.Data?.ToString() ?? "null"}");
            Console.WriteLine($"StatusCode: {balance.StatusCode}");
            Console.WriteLine($"Message: {balance.Message}");
            Console.WriteLine($"Senders: {string.Join(", ", balance.Senders)}");
        }
        else
        {
            Console.WriteLine($"Error: {balance.Message}");
        }
    }
} 