using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Data;
/// <summary>
/// 
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole , string>
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Parking> Parkings { get; set; }
    public DbSet<ParkingSpace> ParkingSpaces { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Car>()
            .HasOne(c => c.User)
            .WithMany(u => u.Cars)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Parking>()
            .HasMany(p => p.ParkingSpaces)
            .WithOne(ps => ps.Parking)
            .HasForeignKey(ps => ps.ParkingId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.ParkingSpace)
            .WithMany()
            .HasForeignKey(r => r.ParkingSpaceId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r=>r.Car)
            .WithMany()
            .HasForeignKey(r=>r.CarId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Car>()
            .HasIndex(c=>c.LicencePlate)
            .IsUnique();

        modelBuilder.Entity<ParkingSpace>()
            .HasIndex(ps => new { ps.ParkingId, ps.IsAvailable });
    }
}