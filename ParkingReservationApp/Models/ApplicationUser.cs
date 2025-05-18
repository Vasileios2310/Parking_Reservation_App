using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ParkingReservationApp.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name must be between 3 and 50 characters")]
    public string Firstname { get; set; }
    
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "last name must be between 3 and 50 characters")]
    public string Lastname { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    
    public ICollection<Car> Cars { get; set; }
    
    public ICollection<Reservation> Reservations { get; set; }
    
    //public ICollection<IdentityRole<string>> UseRoles { get; set; } = new List<IdentityRole<string>>();
}