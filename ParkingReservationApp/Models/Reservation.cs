using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingReservationApp.Models;

/// <summary>
/// Represents a parking reservation in the parking reservation application.
/// </summary>
public class Reservation
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    
    [NotMapped]
    public TimeSpan Duration => EndTime - StartTime;
    
    [Required]
    public int ParkingSpaceId { get; set; }
    public ParkingSpace ParkingSpace { get; set; }
    
    [Required]
    public int CarId { get; set; }
    public Car Car { get; set; }
    
    public bool IsPaid { get; set; }
    
    public bool IsStartNotified { get; set; } = false;
    public bool IsEndNotified { get; set; } = false;
    
    public bool IsOverdue { get; set; } = false;
    public bool IsOverdueCharged { get; set; } = false;
    
    public bool IsCancelled { get; set; } = false;

}