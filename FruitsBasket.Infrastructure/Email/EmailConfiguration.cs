namespace FruitsBasket.Infrastructure.Email;

public class EmailConfiguration
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Sender { get; set; } = null!;
    public string Recipient { get; set; } = null!;
}