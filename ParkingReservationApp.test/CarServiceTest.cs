using Moq;
using Xunit;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using ParkingReservationApp.Data;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;
using ParkingReservationApp.Services;
using Assert = Xunit.Assert;

namespace ParkingReservationApp.test;

/// <summary>
/// 
/// </summary>
public class CarServiceTest
{
    private readonly Mock<ICarRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CarService _service;
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

    public CarServiceTest()
    {
        _repoMock = new Mock<ICarRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<ApplicationDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        
        _service = new CarService(_repoMock.Object, _mapperMock.Object, 
            _contextMock.Object, _httpContextAccessorMock.Object);;
    }

    [Fact]
    public async Task GetByUserId_ReturnsMappedDtos()
    {
        var cars = new List<Car> { new Car { Id = 1 }, new Car { Id = 2 } };
        var dtos = new List<CarDto> { new CarDto { Id = 1 }, new CarDto { Id = 2 } };

        _repoMock.Setup(r => r.GetByUserId("user1")).ReturnsAsync(cars);
        _mapperMock.Setup(m => m.Map<IEnumerable<CarDto>>(cars)).Returns(dtos);

        var result = await _service.GetByUserId("user1");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetById_ReturnsDto_IfFound()
    {
        var car = new Car { Id = 5 };
        var dto = new CarDto { Id = 5 };

        _repoMock.Setup(r => r.GetById(5)).ReturnsAsync(car);
        _mapperMock.Setup(m => m.Map<CarDto?>(car)).Returns(dto);

        var result = await _service.GetById(5);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
    }

    [Fact]
    public async Task Create_Throws_IfPlateExists()
    {
        _repoMock.Setup(r => r.GetByPlate("ABC123")).ReturnsAsync(new Car());

        var dto = new CarDto { LicencePlate = "ABC123", UserId = "user1" };

        await Assert.ThrowsAsync<Exception>(() => _service.Create(dto));
    }

    [Fact]
    public async Task Create_AddsNewCar()
    {
        var dto = new CarDto { Id = 1, LicencePlate = "XYZ999", UserId = "user1" };
        var entity = new Car { Id = 1 };

        _repoMock.Setup(r => r.GetByPlate(dto.LicencePlate)).ReturnsAsync((Car?)null);
        _mapperMock.Setup(m => m.Map<Car>(dto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<CarDto>(entity)).Returns(dto);
        _repoMock.Setup(r => r.Add(entity)).Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.Create(dto);

        Assert.Equal(dto.Id, result.Id);
    }

    [Fact]
    public async Task PlateExists_ReturnsTrue_IfFound()
    {
        _repoMock.Setup(r => r.GetByPlate("ABC123")).ReturnsAsync(new Car());

        var result = await _service.PlateExists("ABC123");

        Assert.True(result);
    }

    [Fact]
    public async Task PlateExists_ReturnsFalse_IfNotFound()
    {
        _repoMock.Setup(r => r.GetByPlate("NOPE")).ReturnsAsync((Car?)null);

        var result = await _service.PlateExists("NOPE");

        Assert.False(result);
    }

    [Fact]
    public async Task CountByUser_ReturnsCount()
    {
        var cars = new List<Car> { new Car { Id = 1 }, new Car { Id = 2 } };
        _repoMock.Setup(r => r.GetByUserId("user2")).ReturnsAsync(cars);

        var result = await _service.CountByUser("user2");

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task Update_ChangesPlate_WhenValid()
    {
        var existing = new Car { Id = 1, LicencePlate = "OLD", UserId = "user1" };
        var dto = new CarDto { Id = 1, LicencePlate = "NEW", UserId = "user1" };

        _repoMock.Setup(r => r.GetById(dto.Id)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.GetByPlate("NEW")).ReturnsAsync((Car?)null);
        _mapperMock.Setup(m => m.Map<CarDto>(existing)).Returns(dto);
        _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.Update(dto);

        Assert.Equal("NEW", result.LicencePlate);
    }

    [Fact]
    public async Task Delete_CallsRepository()
    {
        await _service.Delete(8);

        _repoMock.Verify(r => r.Delete(8), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeletePermanent_RemovesCar_IfExists()
    {
        var car = new Car { Id = 1 };

        var repoMock = new Mock<ICarRepository>();
        repoMock.Setup(r => r.GetById(1)).ReturnsAsync(car);
        repoMock.Setup(r => r.HardDelete(1)).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var mapperMock = new Mock<IMapper>();
        var service = new CarService(repoMock.Object, mapperMock.Object, _contextMock.Object, _httpContextAccessorMock.Object);;

        await service.DeletePermanent(1);

        repoMock.Verify(r => r.HardDelete(1), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePermanent_Throws_IfNotFound()
    {
        var repoMock = new Mock<ICarRepository>();
        repoMock.Setup(r => r.GetById(99)).ReturnsAsync((Car?)null);

        var mapperMock = new Mock<IMapper>();
        var service = new CarService(repoMock.Object, mapperMock.Object , _contextMock.Object, _httpContextAccessorMock.Object);;;

        await Assert.ThrowsAsync<Exception>(() => service.DeletePermanent(99));
    }
}