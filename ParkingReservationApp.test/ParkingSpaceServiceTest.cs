using Moq;
using Xunit;
using AutoMapper;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;
using ParkingReservationApp.Services;
using Assert = Xunit.Assert;

namespace ParkingReservationApp.test;

public class ParkingSpaceServiceTest
{
    private readonly Mock<IParkingSpaceRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ParkingSpaceService _service;

    public ParkingSpaceServiceTest()
    {
        _repoMock = new Mock<IParkingSpaceRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new ParkingSpaceService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedDtos()
    {
        var entities = new List<ParkingSpace> { new ParkingSpace { Id = 1, SpaceNumber = "A1" } };
        var dtos = new List<ParkingSpaceDto> { new ParkingSpaceDto { Id = 1, SpaceNumber = "A1" } };

        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<ParkingSpaceDto>>(entities)).Returns(dtos);

        var result = await _service.GetAll();

        Assert.Single(result);
        Assert.Equal("A1", result.First().SpaceNumber);
    }

    [Fact]
    public async Task GetByParkingId_ReturnsSpaces()
    {
        var parkingId = 10;
        var entities = new List<ParkingSpace>
        {
            new ParkingSpace { Id = 1, ParkingId = parkingId, SpaceNumber = "B1" }
        };
        var dtos = new List<ParkingSpaceDto>
        {
            new ParkingSpaceDto { Id = 1, ParkingId = parkingId, SpaceNumber = "B1" }
        };

        _repoMock.Setup(r => r.GetByParkingId(parkingId)).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<ParkingSpaceDto>>(entities)).Returns(dtos);

        var result = await _service.GetByParkingId(parkingId);

        Assert.Single(result);
        Assert.Equal(parkingId, result.First().ParkingId);
    }

    [Fact]
    public async Task GetById_ReturnsDto()
    {
        var space = new ParkingSpace { Id = 2, SpaceNumber = "C1" };
        var dto = new ParkingSpaceDto { Id = 2, SpaceNumber = "C1" };

        _repoMock.Setup(r => r.GetById(2)).ReturnsAsync(space);
        _mapperMock.Setup(m => m.Map<ParkingSpaceDto?>(space)).Returns(dto);

        var result = await _service.GetById(2);

        Assert.NotNull(result);
        Assert.Equal("C1", result!.SpaceNumber);
    }

    // [Fact]
    // public async Task Create_AddsNewSpace_WhenUnique()
    // {
    //   var dto = new ParkingSpaceDto { ParkingId = 1, SpaceNumber = "D1" };
    //   var entity = new ParkingSpace { ParkingId = 1, SpaceNumber = "D1" };
    //   
    //   _repoMock.Setup(r => r.GetByParkingId(dto.ParkingId)).ReturnsAsync(new List<ParkingSpace>());
    //   _mapperMock.Setup(m => m.Map<ParkingSpace>(dto)).Returns(entity);
    //   _mapperMock.Setup(m => m.Map<ParkingSpaceDto>(entity)).Returns(dto);
    //   
    //   var result = await _service.Create(dto);
    //   
    //   Assert.Equal("D1", result.SpaceNumber);
    //   _repoMock.Verify(r => r.Add(It.IsAny<ParkingSpace>()), Times.Once);
    //   _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
    // }

    [Fact]
    public async Task Create_Throws_WhenDuplicate()
    {
        var dto = new ParkingSpaceDto { ParkingId = 2, SpaceNumber = "D1" };

        var existing = new List<ParkingSpace>
        {
            new ParkingSpace { ParkingId = 2, SpaceNumber = "D1" }
        };

        _repoMock.Setup(r => r.GetByParkingId(dto.ParkingId)).ReturnsAsync(existing);

        await Assert.ThrowsAsync<Exception>(() => _service.Create(dto));
    }

    [Fact]
    public async Task Delete_CallsRepository()
    {
        await _service.Delete(5);

        _repoMock.Verify(r => r.Delete(5), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}