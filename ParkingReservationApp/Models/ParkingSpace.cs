using System.ComponentModel.DataAnnotations;

namespace ParkingReservationApp.Models;

/// <summary>
/// Represents a parking space within a parking facility.
/// </summary>
public class ParkingSpace
{
    public int Id { get; set; }
    
    public bool IsAvailable { get; set; }
    
    [Required]
    [StringLength(10, ErrorMessage = "Space number must be up to 10 characters.")]
    public string SpaceNumber { get; set; }
    
    public int ParkingId { get; set; }
    public Parking Parking { get; set; }
}