using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Data;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;
using Xunit;
using Assert = Xunit.Assert;

namespace ParkingReservationApp.test;

public class ParkingRepositoryTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ParkingRepository _parkingRepository;

    public ParkingRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new ApplicationDbContext(options);
        _parkingRepository = new ParkingRepository(_context);
    }
    
    public void Dispose() => _context.Dispose();

    private void seedParkings()
    {
        _context.Parkings.AddRange(
            new Parking {Name = "Park1" , Area = "Athens" , ContactInfo = "123" , OperatingHours = "9-5"},
            new Parking {Name = "Park2" , Area = "Athens" , ContactInfo = "456" , OperatingHours = "6-2"},
            new Parking {Name = "Park3" , Area = "Lamia" , ContactInfo = "789" , OperatingHours = "24/7"});
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetById_ReturnsCorrectParking()
    {
        var parkings = new Parking {Name = "Test" , Area = "Volos" , ContactInfo = "111" , OperatingHours = "9-5"};
        await _parkingRepository.Add(parkings);
        await _parkingRepository.SaveChangesAsync();
        
        var result = await _parkingRepository.GetById(parkings.Id);
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task GetByArea_ReturnsFilteredParkings()
    {
        seedParkings();
        var result = await _parkingRepository.GetByArea("Athens");
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Delete_RemoveParking()
    {
        var parking = new Parking {Name = "ToDelete" , Area = "Patra" , ContactInfo = "000" , OperatingHours = "10-6"};
        await _parkingRepository.Add(parking);
        await _parkingRepository.SaveChangesAsync();
        
       await _parkingRepository.Delete(parking.Id);
       await _parkingRepository.SaveChangesAsync();
       
       var deletedParking = await _parkingRepository.GetById(parking.Id);
       Assert.Null(deletedParking);
        
    }
}