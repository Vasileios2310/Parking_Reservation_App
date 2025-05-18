using System.ComponentModel.DataAnnotations;

namespace ParkingReservationApp.Models;

public class ParkingSpace
{
    public int Id { get; set; }
    
    [Range(0, 1, ErrorMessage = "IsAvailable must be either 0 (Unavailable) or 1 (Available)")]
    public bool IsAvailable { get; set; }
    public string SpaceNumber { get; set; }
    
    public int ParkingId { get; set; }
    public Parking Parking { get; set; }
}