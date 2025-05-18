namespace ParkingReservationApp.DTOs;

public class PasswordUpdateDto
{
    public string Email { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}