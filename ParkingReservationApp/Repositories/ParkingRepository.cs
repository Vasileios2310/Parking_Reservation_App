using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Data;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;
/// <summary>
/// 
/// </summary>
public class ParkingRepository : IParkingReposiotry
{
    private readonly ApplicationDbContext _context;

    public ParkingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Parking>> GetAll() => await _context.Parkings.ToListAsync();

    public async Task<Parking?> GetById(int id) => await _context.Parkings.FindAsync(id);

    public async Task<IEnumerable<Parking>> GetByArea(string area) => 
        await _context.Parkings.Where(p => p.Area == area).ToListAsync();

    public async Task Add(Parking parking) => await _context.Parkings.AddAsync(parking);

    public async Task Delete(int id)
    {
        var parking = await _context.Parkings.FindAsync(id);
        if (parking != null)
        {
            _context.Parkings.Remove(parking);
        }
    }

    public Task SaveChanges() => _context.SaveChangesAsync();
}