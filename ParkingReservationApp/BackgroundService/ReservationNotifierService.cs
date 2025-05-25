using ParkingReservationApp.Data;
using ParkingReservationApp.Repositories;
using ParkingReservationApp.Services;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Background
{
    public class ReservationNotifierService : BackgroundService
    {
        private readonly ILogger<ReservationNotifierService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ReservationNotifierService(ILogger<ReservationNotifierService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReservationNotifierService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var reservationRepo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var now = DateTime.UtcNow;

                var all = await reservationRepo.GetAllWithUserAndCar(); // must include related data

                // 1. Notify before reservation start
                var upcomingStarts = all
                    .Where(r => !r.IsStartNotified &&
                                r.StartTime > now &&
                                r.StartTime <= now.AddMinutes(60))
                    .ToList();

                foreach (var r in upcomingStarts)
                {
                    if (!string.IsNullOrEmpty(r.User?.Email))
                    {
                        await emailService.SendEmailAsync(
                            r.User.Email,
                            "Upcoming Reservation Reminder",
                            $"Hi {r.User.Firstname},\n\nYour reservation for car {r.Car.LicencePlate} starts at {r.StartTime:t}."
                        );

                        r.IsStartNotified = true;
                        _logger.LogInformation($"[Start Notification] Sent to {r.User.Email}");
                    }
                }

                // 2. Notify before reservation end
                var upcomingEnds = all
                    .Where(r => !r.IsEndNotified &&
                                r.EndTime > now &&
                                r.EndTime <= now.AddMinutes(15))
                    .ToList();

                foreach (var r in upcomingEnds)
                {
                    if (!string.IsNullOrEmpty(r.User?.Email))
                    {
                        await emailService.SendEmailAsync(
                            r.User.Email,
                            "Reservation Ending Soon",
                            $"Hi {r.User.Firstname},\n\nYour reservation will end at {r.EndTime:t}. Please move your car to avoid additional charges."
                        );

                        r.IsEndNotified = true;
                        _logger.LogInformation($"[End Notification] Sent to {r.User.Email}");
                    }
                }

                // 3. Notify overdue reservations
                var overdue = all
                    .Where(r => !r.IsOverdueCharged &&
                                r.EndTime <= now.AddMinutes(-15))
                    .ToList();

                foreach (var r in overdue)
                {
                    r.IsOverdue = true;
                    r.IsOverdueCharged = true;

                    if (!string.IsNullOrEmpty(r.User?.Email))
                    {
                        await emailService.SendEmailAsync(
                            r.User.Email,
                            "Overdue Charge Notice",
                            $"Hi {r.User.Firstname},\n\nYou have not moved your car after the reservation ended at {r.EndTime:t}. An extra charge may apply."
                        );

                        dbContext.AuditLogs.Add(new AuditLog
                        {
                            Action = "Overdue Charge",
                            Entity = "Reservation",
                            Description = $"Overdue charge flagged for reservation #{r.Id}",
                            UserId = r.UserId,
                            Timestamp = DateTime.UtcNow
                        });

                        _logger.LogInformation($"[Overdue Notice] Sent to {r.User.Email}");
                    }
                }

                await reservationRepo.SaveChangesAsync();
                await dbContext.SaveChangesAsync();

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("ReservationNotifierService stopped.");
        }
    }
}
