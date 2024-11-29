using System.Collections.Concurrent;
using System.Data;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Polly;
using PBTPro.DAL.Models;
using PBTPro.Data;

namespace PBTPro.Services;

public class EmailNotificationService : BackgroundService
{
    private const int MaxRetryCount = 3;
    private const int TimerInterval = 30000; // 30 seconds
    private const int EmailSendDelay = 2000;  // 2 seconds

    //private readonly IServiceScopeFactory _serviceScope;
    public IConfiguration Configuration { get; }
    private static readonly ConcurrentQueue<EmailQueueData> EmailQueue = new();

    private Timer _timer;
    private CancellationTokenSource _cts = new();

    //public EmailNotificationService(IServiceScopeFactory serviceScope)
    //{
    //    _serviceScope = serviceScope;
    //}
    public EmailNotificationService(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield(); // Yield to prevent blocking the thread

        _timer = new Timer(ProcessEmails, null, TimerInterval, TimerInterval);

        // Wait until service is stopping
        stoppingToken.WaitHandle.WaitOne();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        await base.StopAsync(cancellationToken);
        _timer?.Dispose();
    }

    public static void EnqueueEmail(EmailQueueData emailData)
    {
        EmailQueue.Enqueue(emailData);
    }

    private async void ProcessEmails(object state)
    {
        while (EmailQueue.TryDequeue(out var email))
        {
            await SendEmail(email, _cts.Token);
            await Task.Delay(EmailSendDelay);
        }
    }

    private async Task SendEmail(EmailQueueData emailData, CancellationToken cancellationToken)
    {
        var configData = GetConfiguration();
        using var message = new MailMessage
        {
            To = { new MailAddress(emailData.ReceiverEmail, emailData.ReceiverName) },
            From = new MailAddress(configData!.smtp_email, configData.smtp_sender),
            Subject = emailData.Subject,
            Body = emailData.Body,
            IsBodyHtml = true
        };

        using var client = new SmtpClient(configData.smtp_host)
        {
            UseDefaultCredentials = false,
            Port = int.Parse(configData.smtp_port),
            Credentials = new NetworkCredential(configData.smtp_user, configData.smtp_password),
            EnableSsl = configData.smtp_protocol
        };

        try
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(MaxRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            await retryPolicy.ExecuteAsync(async () => { await client.SendMailAsync(message); });

        }
        catch (Exception e)
        {
            Console.WriteLine("Exception caught : CreateStatus - {0}", e);
        }
    }

    private email_config GetConfiguration()
    {
        //Get the default email setting
        string strSQL = "SELECT * FROM tbemail WHERE smtpDefault=1 AND rekStatus='A'";
        var configData = new email_config();

        using (MySqlConnection? myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
        {
            using (MySqlCommand? myCmd = new MySqlCommand(strSQL, myConn))
            {
                myConn.Open();
                using (MySqlDataReader? myReaderEmail = myCmd.ExecuteReader())
                {
                    while (myReaderEmail.Read())
                    {
                        configData.smtp_email = myReaderEmail.GetString("smtpEmail");
                        configData.smtp_sender = myReaderEmail.IsDBNull("smtpSender") ? "" : myReaderEmail.GetString("smtpSender");
                        configData.smtp_host = myReaderEmail.GetString("smtpHost");
                        configData.smtp_port = myReaderEmail.IsDBNull("smtpPort") ? "" : myReaderEmail.GetString("smtpPort");
                        configData.smtp_user = myReaderEmail.GetString("smtpUser");
                        configData.smtp_password = myReaderEmail.GetString("smtpPassword");
                        configData.smtp_protocol = myReaderEmail.IsDBNull("smtpProtocol") ? false : myReaderEmail.GetBoolean("smtpProtocol");
                    }
                }
            }
        }

        return configData;
    }
}
