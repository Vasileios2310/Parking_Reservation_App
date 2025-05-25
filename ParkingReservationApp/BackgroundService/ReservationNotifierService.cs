using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ParkingReservationApp.Data;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;
using ParkingReservationApp.Services;

namespace ParkingReservationApp.Background;

public class ReservationNotifierService : BackgroundService
{
    private readonly ILogger<ReservationNotifierService> _logger;
    private readonly IReservationRepository _reservationRepo;
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _context;

    public ReservationNotifierService(
        ILogger<ReservationNotifierService> logger,
        IReservationRepository reservationRepo,
        IEmailService emailService,
        ApplicationDbContext context)
    {
        _logger = logger;
        _reservationRepo = reservationRepo;
        _emailService = emailService;
        _context = context;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        var now = DateTime.UtcNow;

        var all = await _reservationRepo.GetAllWithUserAndCar(); 

        // 1. Notify 1 hour before START
        var upcomingStarts = all
            .Where(r => !r.IsStartNotified &&
                        r.StartTime > now &&
                        r.StartTime <= now.AddMinutes(60))
            .ToList();

        foreach (var r in upcomingStarts)
        {
            if (!string.IsNullOrEmpty(r.User?.Email))
            {
                await _emailService.SendEmailAsync(
                    r.User.Email,
                    "Upcoming Parking Reservation",
                    $"Hi {r.User.Firstname},\n\n" +
                    $"Reminder: Your reservation for car {r.Car.LicencePlate} at {r.ParkingSpace.Parking.Name} " +
                    $"starts at {r.StartTime:t}.");

                r.IsStartNotified = true;
                _logger.LogInformation($"[Start Reminder] Email sent to {r.User.Email}");
            }
        }

        // 2. Notify 15 minutes before END
        var upcomingEnds = all
            .Where(r => !r.IsEndNotified &&
                        r.EndTime > now &&
                        r.EndTime <= now.AddMinutes(15))
            .ToList();

        foreach (var r in upcomingEnds)
        {
            if (!string.IsNullOrEmpty(r.User?.Email))
            {
                await _emailService.SendEmailAsync(
                    r.User.Email,
                    "Remove Your Car Soon",
                    $"Hi {r.User.Firstname},\n\n" +
                    $"Your reservation ends at {r.EndTime:t}. " +
                    $"Please remove your car to avoid extra charges.");

                r.IsEndNotified = true;
                _logger.LogInformation($"[End Reminder] Email sent to {r.User.Email}");
            }
        }
        // 3. Handle overdue reservations (e.g., ended > 15 minutes ago, not yet charged)
        var overdue = all
            .Where(r => !r.IsOverdueCharged &&
                        r.EndTime <= now.AddMinutes(-15)) // ended 15+ minutes ago
            .ToList();

        foreach (var r in overdue)
        {
            r.IsOverdue = true;
            r.IsOverdueCharged = true;

            // Email the user
            if (!string.IsNullOrEmpty(r.User?.Email))
            {
                await _emailService.SendEmailAsync(
                    r.User.Email,
                    "Overdue Charge Notice",
                    $"Hi {r.User.Firstname},\n\n" +
                    $"Your reservation ended at {r.EndTime:t}, but our system detected that your car was not removed in time.\n" +
                    $"An additional charge will be applied as per the terms of service.");

                _logger.LogInformation($"[Overdue Notice] Email sent to {r.User.Email}");
            }

            // Optional: log to audit table
            _context.AuditLogs.Add(new AuditLog
            {
                Action = "Overdue Charge",
                Entity = "Reservation",
                Description = $"Overdue charge applied for Reservation ID {r.Id}",
                UserId = r.UserId,
                Timestamp = DateTime.UtcNow
            });
        }


        await _reservationRepo.SaveChangesAsync();
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    }
}

}