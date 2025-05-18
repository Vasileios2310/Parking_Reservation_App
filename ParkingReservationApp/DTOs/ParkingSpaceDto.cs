namespace ParkingReservationApp.DTOs;

public class ParkingSpaceDto
{
    public int Id { get; set; }
    
    public bool IsAvailable { get; set; }
    public string SpaceNumber { get; set; }
}