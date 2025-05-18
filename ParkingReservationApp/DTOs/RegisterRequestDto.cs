namespace ParkingReservationApp.DTOs;

public class RegisterRequestDto
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<string> LicencePlates { get; set; } = new();
}