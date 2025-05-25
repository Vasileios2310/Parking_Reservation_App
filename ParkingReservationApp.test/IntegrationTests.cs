using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ParkingReservationApp;
using ParkingReservationApp.Data;
using ParkingReservationApp.Services;
using Xunit;
using Assert = Xunit.Assert;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestEmailService _testEmail;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _testEmail = new TestEmailService();
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace real EmailService with test stub
                    var descriptor = services.Single(d => 
                        d.ServiceType == typeof(IEmailService));
                    services.Remove(descriptor);
                    services.AddSingleton<IEmailService>(_testEmail);

                    // Use in-memory DB
                    services.AddDbContext<ApplicationDbContext>(opts =>
                        opts.UseInMemoryDatabase("TestDb"));
                });
            })
            .CreateClient();
    }

    [Fact]
    public async Task Full_Register_Confirm_Login_Flow_Works()
    {
        // 1. Register
        var registerResp = await _client.PostAsJsonAsync("/user/register", new
        {
            firstname = "Jane",
            lastname = "Doe",
            email = "jane@example.com",
            password = "P@ssword1!",
            licencePlates = new[] { "XYZ123" }
        });
        registerResp.EnsureSuccessStatusCode();

        // 2. Capture confirmation link
        Assert.NotEmpty(_testEmail.SentEmails);
        var emailBody = _testEmail.SentEmails.Single().Body;
        // extract userId & token from the link (rudimentary)
        var uri = new Uri(emailBody.Split("confirm-email?")[1]);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var userId = query["userId"];
        var token = query["token"];

        // 3. Confirm email
        var confirmResp = await _client.GetAsync($"/user/confirm-email?userId={userId}&token={System.Net.WebUtility.UrlEncode(token)}");
        confirmResp.EnsureSuccessStatusCode();

        // 4. Login
        var loginResp = await _client.PostAsJsonAsync("/user/login", new
        {
            email = "jane@example.com",
            password = "P@ssword1!"
        });
        loginResp.EnsureSuccessStatusCode();
    }
}

/// <summary>
/// Simple in-memory email capture for testing.
/// </summary>
public class TestEmailService : IEmailService
{
    public struct Sent { public string To, Subject, Body; }
    public readonly List<Sent> SentEmails = new List<Sent>();

    public Task SendEmailAsync(string to, string subject, string body)
    {
        SentEmails.Add(new Sent { To = to, Subject = subject, Body = body });
        return Task.CompletedTask;
    }
}
