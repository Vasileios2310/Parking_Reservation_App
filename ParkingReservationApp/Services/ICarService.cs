using ParkingReservationApp.DTOs;

namespace ParkingReservationApp.Services;

public interface ICarService
{
    Task<IEnumerable<CarDto>> GetByUserId(string userId);
    Task<CarDto?> GetById(int id);
    Task<CarDto?> GetByPlate(string plate);
    Task<CarDto> Create(CarDto dto);
    Task Delete(int id);
    
    Task<bool> PlateExists(string plate);
    Task<int> CountByUser(string userId);
    Task<CarDto> Update(CarDto dto);
    Task DeleteAllByUser(string userId);

}