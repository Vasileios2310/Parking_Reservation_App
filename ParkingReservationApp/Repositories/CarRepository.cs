using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Data;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;

public class CarRepository : ICarRepository
{
    
    private readonly ApplicationDbContext _context;
    
    public CarRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Car>> GetByUserId(string userId) =>
        await _context.Cars.Where(c => c.UserId == userId).ToListAsync();

    public async Task<Car?> GetById(int id) => await _context.Cars.FindAsync(id);

    public async Task<Car?> GetByPlate(string plate) => await _context.Cars.FirstOrDefaultAsync(c => c.LicencePlate == plate);

    public async Task Add(Car car)
    {
        await _context.Cars.AddAsync(car);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car != null)
            _context.Cars.Remove(car);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}