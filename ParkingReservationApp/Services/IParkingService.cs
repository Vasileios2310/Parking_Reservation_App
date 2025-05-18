using ParkingReservationApp.DTOs;

namespace ParkingReservationApp.Services;
/// <summary>
/// 
/// </summary>
public interface IParkingService
{
    Task<IEnumerable<ParkingDto>> GetAllParkings();
    Task<ParkingDto?> GetParkingById(int id);
    Task<IEnumerable<ParkingDto>> GetParkingByArea(string area);
    Task<ParkingDto> CreateParking(ParkingDto parkingDto);
    Task DeleteParking(int id);
}