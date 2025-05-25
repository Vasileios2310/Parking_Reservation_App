namespace ParkingReservationApp.DTOs;

/// <summary>
/// Represents a data transfer object for a parking reservation.
/// </summary>
public class ReservationDto
{
    public int Id { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public string UserId { get; set; }
    public int CarId { get; set; }
    public int ParkingSpaceId { get; set; }
    
    // explicitly mapping LicencePlate --> Car.LicencePlate
    public string LicencePlate { get; set; }
    
    // explicitly mapping ParkingName --> ParkingSpace.Parking.Name
    public string ParkingName { get; set; }
}