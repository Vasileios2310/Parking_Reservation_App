using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;

public interface ICarRepository
{
    Task<IEnumerable<Car>> GetByUserId(string userId);
    Task<Car?> GetById(int id);
    Task<Car?> GetByPlate(string plate);
    Task Add(Car car);
    Task Delete(int id);
    Task SaveChangesAsync();
}