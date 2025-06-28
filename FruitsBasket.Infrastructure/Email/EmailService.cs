using System.Net;
using System.Net.Mail;
using FruitsBasket.Model.Fruit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FruitsBasket.Infrastructure.Email;

public class EmailService(IOptions<EmailConfiguration> configuration, ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailConfiguration _configuration = configuration.Value;

    public async Task SendFruitNotificationAsync(FruitDto fruit, string action)
    {
        var recipientEmail = _configuration.Recipient;
        var subject = $"fruits-basket | {action}({fruit.Id})";
        var body = $"""
                    Info:
                    Action: {action}
                    Timestamp: {DateTime.UtcNow} UTC

                    Data:
                    Id: {fruit.Id}
                    Name: {fruit.Name}
                    Weight: {fruit.Weight}
                    HarvestDate: {fruit.HarvestDate}
                    """;

        try
        {
            await SendEmailAsync(recipientEmail, subject, body);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "Failed to send email notification for fruit {FruitName} (ID: {FruitId}) - Action: {Action}",
                fruit.Name, fruit.Id, action);
        }

        logger.LogInformation("Email notification sent for fruit {FruitName} (ID: {FruitId}) - Action: {Action}",
            fruit.Name, fruit.Id, action);
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_configuration.Host, _configuration.Port);
        client.Credentials = new NetworkCredential(_configuration.Username, _configuration.Password);
        client.EnableSsl = true;

        var message = new MailMessage(_configuration.Sender, to, subject, body);

        await client.SendMailAsync(message);
    }
}