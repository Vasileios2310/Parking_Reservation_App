using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;

/// <summary>
/// Interface for handling operations related to reservations within the application.
/// </summary>
public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAll();
    Task<Reservation?> GetById(int id);
    Task<IEnumerable<Reservation>> GetByUserId(string userId);
    Task Add(Reservation reservation);
    Task Delete(int id);
    Task SaveChangesAsync();
}