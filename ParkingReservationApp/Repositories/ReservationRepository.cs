using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Data;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;

/// <summary>
/// Provides methods for accessing and manipulating reservation data in the database.
/// </summary>
public class ReservationRepository : IReservationRepository
{
    
    private readonly ApplicationDbContext _context;
    
    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Reservation>> GetAll() => 
        await _context.Reservations
            .Include(r => r.Car)
            .Include(r => r.ParkingSpace)
                .ThenInclude(ps => ps.Parking)
            .ToListAsync();

    public async Task<Reservation?> GetById(int id) =>
         await _context.Reservations
             .Include(r => r.Car)
             .Include(r => r.ParkingSpace)
                .ThenInclude(ps => ps.Parking)
             .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Reservation>> GetByUserId(string userId) =>
        await _context.Reservations
            .Where(r => r.UserId == userId)
            .Include(r => r.Car)
            .Include(r => r.ParkingSpace)
            .ThenInclude(ps => ps.Parking)
            .ToListAsync();

    public async Task Add(Reservation reservation) => await _context.Reservations.AddAsync(reservation);

    public async Task Delete(int id)
    {
        var res = await _context.Reservations.FindAsync(id);
        if(res != null)
            _context.Reservations.Remove(res);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
    
    public async Task<IEnumerable<Reservation>> GetAllWithUserAndCar()
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Car)
            .Include(r => r.ParkingSpace)
            .ThenInclude(ps => ps.Parking)
            .Where(r => r.StartTime > DateTime.UtcNow.AddHours(-1)) // optional filter for optimization
            .ToListAsync();
    }
}