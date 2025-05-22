using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Data;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;

public class ParkingSpaceRepository : IParkingSpaceRepository
{
    private readonly ApplicationDbContext _context;
    
    public ParkingSpaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ParkingSpace>> GetAll() => await _context.ParkingSpaces.ToListAsync();

    public async Task<IEnumerable<ParkingSpace>> GetByParkingId(int parkingId) => 
        await _context.ParkingSpaces.Where(p => p.ParkingId == parkingId).ToListAsync();

    public async Task<ParkingSpace?> GetById(int id) => await _context.ParkingSpaces.FindAsync(id);

    public async Task Add(ParkingSpace parkingSpace) => await _context.ParkingSpaces.AddAsync(parkingSpace);

    public async Task Delete(int id)
    {
        var space = await GetById(id);
        if (space != null)
        {
            _context.ParkingSpaces.Remove(space);
        }
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}