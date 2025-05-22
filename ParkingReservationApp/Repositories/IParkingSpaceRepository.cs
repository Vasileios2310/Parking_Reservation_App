using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;

public interface IParkingSpaceRepository
{
    Task<IEnumerable<ParkingSpace>> GetAll();
    Task<IEnumerable<ParkingSpace>> GetByParkingId(int parkingId);
    Task<ParkingSpace?> GetById(int id);
    Task Add(ParkingSpace parkingSpace);
    Task Delete(int id);
    Task SaveChangesAsync();
}