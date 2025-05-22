namespace ParkingReservationApp.DTOs;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a parking space.
/// </summary>
/// <remarks>
/// This class is used to encapsulate data related to a parking space including
/// its unique identifier, availability status, space number, and associated parking entity identifier.
/// </remarks>
public class ParkingSpaceDto
{
    public int Id { get; set; }
    
    public bool IsAvailable { get; set; }
    public string SpaceNumber { get; set; }
    public int ParkingId { get; set; }
}