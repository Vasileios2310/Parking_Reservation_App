namespace ParkingReservationApp.test;

using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;
using ParkingReservationApp.Services;


public class ParkingServiceTest
{
    private readonly Mock<IParkingRepository> _repoMock;
    private readonly ParkingService _parkingService;
    private readonly Mock<IMapper> _mapperMock;

    public ParkingServiceTest()
    {
        _repoMock = new Mock<IParkingRepository>();
        _mapperMock = new Mock<IMapper>();
        _parkingService = new ParkingService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllParkings_ReturnsMapperDtos()
    {
        var parkings = new List<Parking>
        {
            new Parking { Id = 1, Name = "Parking 1" } };
            var dtos = new List<ParkingDto> { new ParkingDto { Id = 1, Name = "Parking 1" } };
            
            _repoMock.Setup(r => r.GetAll()).ReturnsAsync(parkings);
            _mapperMock.Setup(m => m.Map<IEnumerable<ParkingDto>>(parkings)).Returns(dtos);
            
            var result = await _parkingService.GetAllParkings();
            
            Assert.Single(result);
            Assert.Equal("Parking 1", result.First().Name);
    }

    [Fact]
    public async Task GetParkingById_ReturnsMapperDto()
    {
        var parking = new Parking { Id = 1, Name = "Parking 1" };
        var dto = new ParkingDto { Id = 1, Name = "Parking 1" };
        
        _repoMock.Setup(p => p.GetById(1)).ReturnsAsync(parking);
        _mapperMock.Setup(m => m.Map<ParkingDto?>(parking)).Returns(dto);
        
        var result = await _parkingService.GetParkingById(1);
        
        Assert.NotNull(result);
        Assert.Equal("Parking 1", result!.Name);
    }
    
    [Fact]
    public async Task CreateParking_AddsAndReturnsDto()
    {
        var dto = new ParkingDto { Name = "New" };
        var entity = new Parking { Name = "New" };

        _mapperMock.Setup(m => m.Map<Parking>(dto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<ParkingDto>(entity)).Returns(dto);

        var result = await _parkingService.CreateParking(dto);

        Assert.Equal("New", result.Name);
        _repoMock.Verify(r => r.Add(It.IsAny<Parking>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}