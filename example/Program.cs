using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IletiMerkezi;
using IletiMerkezi.Http;

class Program
{
    // ── Kimlik bilgileri ──────────────────────────────────────────────────────
    const string API_KEY    = "api-key";
    const string API_HASH   = "api-secret";
    const string DEFAULT_SENDER = "ONAYLI BASLIK";

    // ── Test telefon numaraları ───────────────────────────────────────────────
    const string PHONE_1 = "505xxxxxxx";
    const string PHONE_2 = "533xxxxxxx";

    // ── Test sipariş ID'leri ─────────────────────────────────────────────────
    const int ORDER_ID         = 123;
    const int ORDER_ID_PAGED   = 123;
    const int ORDER_ID_CANCEL  = 123;

    static async Task Main(string[] args)
    {
        var client = new IletiMerkeziClient(API_KEY, API_HASH, DEFAULT_SENDER);

        /**
        * Account servisi örnek kullanımları
        */
        // await GetBalanceAsync(client);

        /**
        * Blacklist servisi örnek kullanımları
        */
        // await BlacklistCreateAsync(client, PHONE_1);
        // await BlacklistListAsync(client);
        // await BlacklistListWithDateAsync(client, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
        // await BlacklistListWithPaginationAsync(client, page: 2, rowCount: 10);
        // await BlacklistDeleteAsync(client, PHONE_1);

        /**
        * Report servisi örnek kullanımları
        */
        // Temel rapor sorgusu: sadece orderId
        // await ReportGetAsync(client, orderId: ORDER_ID);

        // Sayfalama ile rapor sorgusu
        // await ReportGetWithPaginationAsync(client, orderId: ORDER_ID_PAGED, page: 7, rowCount: 1000);

        // Sonraki sayfayı getir (GetAsync çağrısından sonra kullanılabilir)
        // await ReportGetNextAsync(client, orderId: ORDER_ID);

        /**
        * Sender servisi örnek kullanımları
        */
        // await SenderListAsync(client);

        /**
        * SMS servisi örnek kullanımları
        */
        // Tek numaraya SMS gönder
        // await SmsSendAsync(client, recipient: PHONE_1, message: "Merhaba, bu bir test mesajıdır.");

        // Tek numara, özel gönderici adıyla
        // await SmsSendAsync(client, recipient: PHONE_1, message: "Merhaba!", sender: "iletiMrkezi");

        // Birden fazla numaraya aynı mesajı gönder
        // await SmsSendToMultipleAsync(client, recipients: new[] { PHONE_1, PHONE_2 }, message: "Toplu mesaj testi.");

        // Zamanlanmış SMS gönder (ileri tarihli)
        // await SmsSendScheduledAsync(client, recipient: PHONE_1, message: "Zamanlanmış mesaj 1.", sendDateTime: "2026-12-01 10:00:00");

        // IYS onayı kapalı olarak gönder
        // await SmsSendWithIysDisabledAsync(client, recipient: PHONE_1, message: "IYS kapalı mesaj.");

        // IYS listesi TACIR olarak gönder
        // await SmsSendWithIysListAsync(client, recipient: PHONE_1, message: "Tacir listesi mesajı.", iysList: "TACIR");

        // Her numaraya farklı mesaj gönder (bulk)
        // await SmsSendBulkAsync(client);

        // Siparişi iptal et
        // await SmsCancelAsync(client, orderId: ORDER_ID_CANCEL);

        /**
        * Summary servisi örnek kullanımları
        */
        // Tarih aralığıyla sipariş özeti listele
        // await SummaryListAsync(client, startDate: "2026-01-01", endDate: "2026-01-10");

        // Sayfalama ile özet listele
        // await SummaryListAsync(client, startDate: "2026-01-01", endDate: "2026-01-10", page: 3);

        // Sonraki sayfayı getir (ListAsync çağrısından sonra kullanılabilir)
        // await SummaryListNextAsync(client, startDate: "2026-01-01", endDate: "2026-01-01");

        /**
        * Webhook servisi örnek kullanımları
        */
        // Gelen webhook verisini parse et
        // WebhookHandleAsync(client);
    }

    static void WebhookHandleAsync(IletiMerkeziClient client)
    {
        // Gerçek kullanımda bu JSON, iletimerkezi.com'un POST ettiği webhook body'sinden gelir.
        var webhookJson = $@"{{
            ""report"": {{
                ""id"": 123456,
                ""packet_id"": 789,
                ""status"": ""delivered"",
                ""to"": ""{PHONE_1}"",
                ""body"": ""Test mesajı""
            }}
        }}";

        var webhookService = client.Webhook();
        var report = webhookService.Handle(webhookJson);

        Console.WriteLine("=== Webhook Raporu ===");
        Console.WriteLine($"  ID         : {report.Id}");
        Console.WriteLine($"  Packet ID  : {report.PacketId}");
        Console.WriteLine($"  Alıcı      : {report.To}");
        Console.WriteLine($"  Mesaj      : {report.Body}");
        Console.WriteLine($"  Durum      : {report.Status}");
        Console.WriteLine($"  İletildi mi: {report.IsDelivered}");
        Console.WriteLine($"  Kabul mü   : {report.IsAccepted}");
        Console.WriteLine($"  İletilemedi: {report.IsUndelivered}");
    }

    static async Task GetBalanceAsync(IletiMerkeziClient client)
    {
        var accountService = client.Account();
        var response = await accountService.GetBalanceAsync();

        Console.WriteLine(client.Debug());

        if (response.Ok)
        {
            Console.WriteLine($"{response.Amount} TL || {response.Credits} SMS krediniz kaldı.");
        }
        else
        {
            Console.WriteLine($"Hata: {response.Message}");
        }
    }

    static async Task SenderListAsync(IletiMerkeziClient client)
    {
        var senderService = client.Senders();
        var response = await senderService.ListAsync();

        Console.WriteLine(client.Debug());

        if (response.Ok)
        {
            Console.WriteLine($"Onaylı gönderici sayısı: {response.Senders.Count}");
            Console.WriteLine($"Göndericiler: {string.Join(", ", response.Senders)}");
        }
        else
        {
            Console.WriteLine($"Hata: {response.Message}");
        }
    }

    // Tek numaraya SMS gönder
    static async Task SmsSendAsync(IletiMerkeziClient client, string recipient, string message, string sender = null)
    {
        var smsService = client.Sms();
        var response = await smsService.SendAsync(recipient, message, sender);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"SMS gönderildi. Sipariş ID: {response.OrderId}");
        else
            Console.WriteLine($"Hata: {response.Message}");
    }

    // Birden fazla numaraya aynı mesajı gönder
    static async Task SmsSendToMultipleAsync(IletiMerkeziClient client, IEnumerable<string> recipients, string message, string sender = null)
    {
        var smsService = client.Sms();
        var response = await smsService.SendAsync(recipients, message, sender);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"Toplu SMS gönderildi. Sipariş ID: {response.OrderId}");
        else
            Console.WriteLine($"Hata: {response.Message}");
    }

    // Zamanlanmış SMS gönder
    static async Task SmsSendScheduledAsync(IletiMerkeziClient client, string recipient, string message, string sendDateTime, string sender = null)
    {
        var smsService = client.Sms().Schedule(sendDateTime);
        var response = await smsService.SendAsync(recipient, message, sender);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"Zamanlanmış SMS oluşturuldu. Sipariş ID: {response.OrderId} | Gönderim: {sendDateTime}");
        else
            Console.WriteLine($"Hata: {response.Message}");
    }

    // IYS onayı kapalı SMS gönder
    static async Task SmsSendWithIysDisabledAsync(IletiMerkeziClient client, string recipient, string message, string sender = null)
    {
        var smsService = client.Sms().DisableIysConsent();
        var response = await smsService.SendAsync(recipient, message, sender);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"SMS gönderildi (IYS kapalı). Sipariş ID: {response.OrderId}");
        else
            Console.WriteLine($"Hata: {response.Message}");
    }

    // IYS listesi belirterek SMS gönder (BIREYSEL / TACIR)
    static async Task SmsSendWithIysListAsync(IletiMerkeziClient client, string recipient, string message, string iysList, string sender = null)
    {
        var smsService = client.Sms().SetIysList(iysList);
        var response = await smsService.SendAsync(recipient, message, sender);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"SMS gönderildi (IYS liste: {iysList}). Sipariş ID: {response.OrderId}");
        else
            Console.WriteLine($"Hata: {response.Message}");
    }

    // Her numaraya farklı mesaj gönder (bulk)
    static async Task SmsSendBulkAsync(IletiMerkeziClient client, string sender = null)
    {
        var recipientMessages = new Dictionary<string, string>
        {
            { PHONE_1, "Merhaba Ahmet, siparişiniz hazır." },
            { PHONE_2, "Merhaba Serkan, kampanyamızdan haberdar olun." }
        };

        var smsService = client.Sms();
        var response = await smsService.SendBulkAsync(recipientMessages, sender);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"Kişiselleştirilmiş toplu SMS gönderildi. Sipariş ID: {response.OrderId}");
        else
            Console.WriteLine($"Hata: {response.Message}");
    }

    static async Task SummaryListAsync(IletiMerkeziClient client, string startDate, string endDate, int page = 1)
    {
        var summaryService = client.Summary();
        var response = await summaryService.ListAsync(startDate, endDate, page);

        Console.WriteLine(client.Debug());
        PrintSummary(response, label: $"[{startDate} - {endDate}, Sayfa: {page}]");
    }

    static async Task SummaryListNextAsync(IletiMerkeziClient client, string startDate, string endDate)
    {
        var summaryService = client.Summary();

        var current = await summaryService.ListAsync(startDate, endDate, page: 1);
        int page = 1;
        PrintSummary(current, label: $"--- {page}. Sayfa ---");

        while (current.Ok && current.Orders != null && current.Orders.Count > 0)
        {
            try
            {
                current = await summaryService.NextAsync();
                page++;
                Console.WriteLine(client.Debug());
                PrintSummary(current, label: $"--- {page}. Sayfa ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NextAsync hata verdi, döngü durduruluyor: {ex.Message}");
                break;
            }
        }
    }

    static void PrintSummary(IletiMerkezi.Models.SummaryResponse response, string label = null)
    {
        if (label != null)
            Console.WriteLine(label);

        if (!response.Ok)
        {
            Console.WriteLine($"Hata: {response.Message}");
            return;
        }

        Console.WriteLine($"Toplam sipariş sayısı: {response.Count}");

        foreach (var order in response.Orders)
        {
            Console.WriteLine($"  Sipariş ID   : {order.Id}");
            Console.WriteLine($"  Durum        : {order.StatusText} ({order.Status})");
            Console.WriteLine($"  Gönderici    : {order.Sender}");
            Console.WriteLine($"  Oluşturma    : {order.SubmitAt}");
            Console.WriteLine($"  Gönderim     : {order.SendAt}");
            Console.WriteLine($"  Toplam       : {order.Total}");
            Console.WriteLine($"  İletilen     : {order.Delivered}");
            Console.WriteLine($"  İletilemeyen : {order.Undelivered}");
            Console.WriteLine($"  Bekleyen     : {order.Waiting}");
            Console.WriteLine();
        }
    }

    // Siparişi iptal et
    static async Task SmsCancelAsync(IletiMerkeziClient client, int orderId)
    {
        var smsService = client.Sms();
        var response = await smsService.CancelAsync(orderId);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"Sipariş iptal edildi. ID: {orderId}");
        else
            Console.WriteLine($"Hata: {response.Message}");
    }

    static async Task BlacklistCreateAsync(IletiMerkeziClient client, string number)
    {
        var blacklistService = client.Blacklist();
        var response = await blacklistService.CreateAsync(number);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"Kara listeye eklendi: {number}");
        else
            Console.WriteLine($"Hata: {response.Message} StatusCode: {response.StatusCode}");
    }

    static async Task BlacklistListAsync(IletiMerkeziClient client)
    {
        var blacklistService = client.Blacklist();
        var response = await blacklistService.ListAsync();

        Console.WriteLine(client.Debug());

        if (response.Ok)
        {
            Console.WriteLine($"Toplam kara liste sayısı: {response.Count}");
            Console.WriteLine($"Numaralar: {string.Join(", ", response.Numbers)}");
        }
        else
        {
            Console.WriteLine($"Hata: {response.Message}");
        }
    }

    static async Task BlacklistListWithDateAsync(IletiMerkeziClient client, DateTime startDate, DateTime endDate)
    {
        var blacklistService = client.Blacklist();
        var response = await blacklistService.ListAsync(startDate: startDate, endDate: endDate);

        Console.WriteLine(client.Debug());

        if (response.Ok)
        {
            Console.WriteLine($"[{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}] Toplam: {response.Count}");
            Console.WriteLine($"Numaralar: {string.Join(", ", response.Numbers)}");
        }
        else
        {
            Console.WriteLine($"Hata: {response.Message}");
        }
    }

    static async Task BlacklistListWithPaginationAsync(IletiMerkeziClient client, int page, int rowCount)
    {
        var blacklistService = client.Blacklist();
        var response = await blacklistService.ListAsync(page: page, rowCount: rowCount);

        Console.WriteLine(client.Debug());

        if (response.Ok)
        {
            Console.WriteLine($"[Sayfa: {page}, Satır: {rowCount}] Toplam: {response.Count}");
            Console.WriteLine($"Numaralar: {string.Join(", ", response.Numbers)}");
        }
        else
        {
            Console.WriteLine($"Hata: {response.Message}");
        }
    }

    static async Task BlacklistDeleteAsync(IletiMerkeziClient client, string number)
    {
        var blacklistService = client.Blacklist();
        var response = await blacklistService.DeleteAsync(number);

        Console.WriteLine(client.Debug());

        if (response.Ok)
            Console.WriteLine($"Kara listeden silindi: {number}");
        else
            Console.WriteLine($"Hata: {response.Message}");
    }

    // Temel rapor sorgusu: sadece orderId ile
    static async Task ReportGetAsync(IletiMerkeziClient client, int orderId)
    {
        var reportService = client.Reports();
        var response = await reportService.GetAsync(orderId);

        Console.WriteLine(client.Debug());
        PrintReport(response);
    }

    // Sayfalama ile rapor sorgusu: page ve rowCount parametreleriyle
    static async Task ReportGetWithPaginationAsync(IletiMerkeziClient client, int orderId, int page, int rowCount)
    {
        var reportService = client.Reports();
        var response = await reportService.GetAsync(orderId, page: page, rowCount: rowCount);

        Console.WriteLine(client.Debug());
        PrintReport(response, label: $"[Sayfa: {page}, Satır: {rowCount}]");
    }

    // Sonraki sayfa sorgusu: GetAsync'ten sonra NextAsync ile devam
    static async Task ReportGetNextAsync(IletiMerkeziClient client, int orderId)
    {
        var reportService = client.Reports();

        var first = await reportService.GetAsync(orderId, page: 1, rowCount: 10);
        PrintReport(first, label: "--- 1. Sayfa ---");

        var next = await reportService.NextAsync();
        Console.WriteLine(client.Debug());
        PrintReport(next, label: "--- 2. Sayfa ---");
    }

    static void PrintReport(IletiMerkezi.Models.ReportResponse response, string label = null)
    {
        if (label != null)
            Console.WriteLine(label);

        if (!response.Ok)
        {
            Console.WriteLine($"Hata: {response.Message}");
            return;
        }

        Console.WriteLine($"Sipariş ID   : {response.Id}");
        Console.WriteLine($"Durum        : {response.StatusText} ({response.Status})");
        Console.WriteLine($"Gönderici    : {response.Sender}");
        Console.WriteLine($"Oluşturma    : {response.SubmitAt}");
        Console.WriteLine($"Gönderim     : {response.SendAt}");
        Console.WriteLine($"Toplam       : {response.Total}");
        Console.WriteLine($"İletilen     : {response.Delivered}");
        Console.WriteLine($"İletilemeyen : {response.Undelivered}");
        Console.WriteLine($"Bekleyen     : {response.Waiting}");
        Console.WriteLine($"Mesaj sayısı : {response.Messages.Count}");
        foreach (var msg in response.Messages)
            Console.WriteLine($"  Numara: {msg.Number} | Durum: {msg.StatusText} ({msg.Status})");
    }
} 