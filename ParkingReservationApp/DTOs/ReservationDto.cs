namespace ParkingReservationApp.DTOs;

public class ReservationDto
{
    public int Id { get; set; }
    
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
    
    // explicitly mapping LicencePlate --> Car.LicencePlate
    public string LicencePlate { get; set; }
    
    // explicitly mapping ParkingName --> ParkingSpace.Parking.Name
    public string ParkingName { get; set; }
    
    public bool IsPaid { get; set; }
}