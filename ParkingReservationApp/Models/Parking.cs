using System.ComponentModel.DataAnnotations;

namespace ParkingReservationApp.Models;
/// <summary>
/// 
/// </summary>
public class Parking
{
    public int Id { get; set; } 
    [Required (ErrorMessage ="You must enter a name")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 20 characters")]
    public string Name { get; set; }
    
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Area must contain only letters and spaces")]
    public string Area { get; set; }
    
    [Required (ErrorMessage ="You must enter contact information")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Contact information must be between 3 and 50 characters")]
    public string ContactInfo { get; set; }
    
    [Required (ErrorMessage ="You must enter operating hours")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Operating hours must be between 3 and 50 characters")]
    public string OperatingHours { get; set; }
    
    public ICollection<ParkingSpace> ParkingSpaces { get; set; }
}