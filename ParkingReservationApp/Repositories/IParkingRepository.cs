using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;
/// <summary>
/// 
/// </summary>
public interface IParkingRepository
{
    Task<IEnumerable<Parking>> GetAll();
    Task<Parking?> GetById(int id);
    Task<IEnumerable<Parking>> GetByArea(string area);
    Task Add(Parking parking);
    Task Delete(int id);
    Task SaveChangesAsync();
}