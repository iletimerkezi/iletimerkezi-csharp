# İletiMerkezi C# Kütüphanesi

İletiMerkezi SMS API'sini kullanarak SMS gönderimi yapmanızı sağlayan resmi C# kütüphanesidir.

## Desteklenen Platformlar

- .NET Standard 2.0+
- .NET Core 2.0+
- .NET Framework 4.6.1+
- .NET 5.0+
- .NET 6.0+

## Kurulum

### NuGet Paketi ile Kurulum

[Nuget Paket Linki](https://www.nuget.org/packages/iletimerkezi/)

```bash
dotnet add package iletimerkezi
```

veya Package Manager Console kullanarak:

```bash
Install-Package iletimerkezi
```


## Kullanım

### Client Oluşturma

```csharp
var client = new IletiMerkeziClient(
    "API_KEY",
    "API_HASH",
    "ONAYLI BASLIK" // İsteğe bağlı
);
```

### SMS Gönderimi

```csharp
// Tek alıcıya SMS gönderimi
var smsService = client.Sms();
var response = await smsService.SendAsync(
    "50570xxxxx",
    "Merhaba, bu bir test mesajıdır."
);

if (response.Ok)
{
    Console.WriteLine($"SMS gönderildi. {response.OrderId}");
}
else
{
    Console.WriteLine($"Hata: {response.Message}");
}


// Çoklu alıcıya SMS gönderimi
var recipients = new List<string> {
    "50570xxxxx",
    "50570xxxx1"
};
var response = await smsService.SendAsync(
    recipients,
    "Merhaba, bu bir toplu test mesajıdır."
);


// Çoklu alıcıya, farklı mesajlar gönderimi
var response = await smsService.SendBulkAsync(
    recipients,
    new Dictionary<string, string> {
        { "50570xxxx1", "Test mesajı 1" },
        { "50570xxxx2", "Test mesajı 2" }
    }
);
```

### İleri Tarihli SMS Gönderimi
```csharp
var response = await client.Sms()
    .Schedule("2024-03-28 15:00:00")
    .SendAsync("50570xxxx1", "İleri tarihli test mesajı");
```

### IYS Onaylı SMS Gönderimi
```csharp
var response = await client.Sms()
    .SetIysList("TACIR") // veya "BIREYSEL"
    .SendAsync("50570xxxx1", "IYS onaylı test mesajı");
```

### IYS Üzerinden SMS Onayı aramadan gönderim
```csharp
var response = await client.Sms()
    .DisableIysConsent()
    .SetIysList("TACIR") // veya "BIREYSEL"
    .SendAsync("50570xxxx1", "IYS onaylı test mesajı");
```

### Bakiye Sorgulama
```csharp
var accountService = client.Account();
var balance = await accountService.GetBalanceAsync();
if (balance.Ok)
{
    Console.WriteLine($"Bakiye: {balance.Amount} TL");
    Console.WriteLine($"Kalan Kredi: {balance.Credits}");
}
```

### Gönderici Adları Listesi
```csharp
var senderService = client.Senders();
var senders = await senderService.ListAsync();
if (senders.Ok)
{
    foreach (var sender in senders.Senders)
    {
        Console.WriteLine($"Gönderici Adı: {sender}");
    }
}
``` 

### Rapor Alma
```csharp
var reportService = client.Reports();
var report = await reportService.GetAsync(orderId);

if (report.Ok)
{
    Console.WriteLine($"Durum: {report.Status}");
    Console.WriteLine($"Gönderim Zamanı: {report.SendDate}");
    Console.WriteLine($"İletilen: {report.Delivered}");
    Console.WriteLine($"İletilemeyen: {report.Undelivered}");
}
```

### Webhook İşleme
```csharp
var webhookService = new WebhookService();
var webhook = webhookService.HandleJson(webhookData);
Console.WriteLine($"Rapor ID: {webhook.Report.Id}");
Console.WriteLine($"Durum: {webhook.Report.Status}");
Console.WriteLine($"Alıcı: {webhook.Report.To}");
```


## Geliştirme

### Projeyi Build Etme
```bash
dotnet build
```

### Testleri Çalıştırma
```bash
dotnet test
``` 