using System.ComponentModel.DataAnnotations;

namespace ParkingReservationApp.Models;

public class Reservation
{
    public int Id { get; set; }
    
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    
    public int ParkingSpaceId { get; set; }
    public ParkingSpace ParkingSpace { get; set; }
    
    public int CarId { get; set; }
    public Car Car { get; set; }
    
    public bool IsPaid { get; set; }
}