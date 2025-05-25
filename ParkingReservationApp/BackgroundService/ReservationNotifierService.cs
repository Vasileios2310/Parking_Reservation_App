using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ParkingReservationApp.Repositories;

namespace ParkingReservationApp.Background;

/// <summary>
/// A background service that periodically checks reservations and sends notifications for reservations
/// starting within the next hour but not yet paid.
/// </summary>
public class ReservationNotifierService : BackgroundService
{
    private readonly ILogger<ReservationNotifierService> _logger;
    private readonly IReservationRepository _reservationRepo;

    public ReservationNotifierService(
        ILogger<ReservationNotifierService> logger,
        IReservationRepository reservationRepo)
    {
        _logger = logger;
        _reservationRepo = reservationRepo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var all = await _reservationRepo.GetAll();

            var upcoming = all
                .Where(r =>
                    !r.IsPaid &&
                    r.StartTime > DateTime.Now &&
                    r.StartTime <= DateTime.Now.AddMinutes(60))
                .ToList();

            foreach (var reservation in upcoming)
            {
                _logger.LogInformation(
                    $"[🔔 Notification] User {reservation.UserId} has a reservation in under 1 hour for car {reservation.Car?.LicencePlate ?? reservation.CarId.ToString()}.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Check every 5 minutes
        }
    }
}