namespace ParkingReservationApp.Models;

using System.ComponentModel.DataAnnotations;

public enum UserRole
{
    [Display(Name = "Admin")]
    Admin,

    [Display(Name = "Customer")]
    Customer,

    [Display(Name = "Manager")]
    Manager
}
