using ParkingReservationApp.DTOs;

namespace ParkingReservationApp.Services;

/// <summary>
/// Defines the contract for the service responsible for managing parking spaces.
/// </summary>
public interface IParkingSpaceService
{
    Task<IEnumerable<ParkingSpaceDto>> GetAll();
    Task<IEnumerable<ParkingSpaceDto>> GetByParkingId(int parkingId);
    Task<ParkingSpaceDto?> GetById(int id);
    Task<ParkingSpaceDto> Create(ParkingSpaceDto dto);
    Task Delete(int id);
}