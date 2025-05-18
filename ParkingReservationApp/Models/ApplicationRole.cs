using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ParkingReservationApp.Models;
/// <summary>
/// 
/// </summary>
public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}