using System.ComponentModel.DataAnnotations;

namespace ParkingReservationApp.Models;
/// <summary>
/// Represents a user's car with a licence plate.
/// </summary>
public class Car
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Licence Plate is required")]
    [StringLength(8, MinimumLength = 3, ErrorMessage = "Licence Plate must be between 3 and 8 characters")]
    [RegularExpression(@"^[A-Z]{3}[0-9]{4}]$", ErrorMessage = "Licence Plate must contain only letters and numbers")]
    public string LicencePlate { get; set; }

    
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}