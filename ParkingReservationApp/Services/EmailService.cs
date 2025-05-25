using System.Net;
using System.Net.Mail;

namespace ParkingReservationApp.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var from = _configuration["Email:From"];
        var password = _configuration["Email:Password"];
        var smtpHost = _configuration["Email:SmtpHost"];
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(from, password),
            EnableSsl = true
        };
        
        var message = new MailMessage(from, to, subject, body);
        await client.SendMailAsync(message);
    }
}