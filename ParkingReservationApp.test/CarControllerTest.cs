using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ParkingReservationApp.Controllers;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Services;
using Assert = Xunit.Assert;

namespace ParkingReservationApp.test;

/// <summary>
/// 
/// </summary>
public class CarControllerTest
{
    private readonly Mock<ICarService> _carServiceMock;
    private readonly CarController _controller;

    public CarControllerTest()
    {
        _carServiceMock = new Mock<ICarService>();
        _controller = new CarController(_carServiceMock.Object);
    }

    [Fact]
    public async Task GetByUser_ReturnsOk_WithCars()
    {
        var cars = new List<CarDto> { new CarDto { Id = 1 }, new CarDto { Id = 2 } };
        _carServiceMock.Setup(s => s.GetByUserId("user1")).ReturnsAsync(cars);

        var result = await _controller.GetByUser("user1");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(cars, ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_IfNull()
    {
        _carServiceMock.Setup(s => s.GetById(10)).ReturnsAsync((CarDto?)null);

        var result = await _controller.GetById(10);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByPlate_ReturnsCar_IfFound()
    {
        var dto = new CarDto { Id = 3, LicencePlate = "XYZ123" };
        _carServiceMock.Setup(s => s.GetByPlate("XYZ123")).ReturnsAsync(dto);

        var result = await _controller.GetByPlate("XYZ123");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task PlateExists_ReturnsTrue_WhenExists()
    {
        _carServiceMock.Setup(s => s.PlateExists("ABC123")).ReturnsAsync(true);

        var result = await _controller.PlateExists("ABC123");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)ok.Value!);
    }

    [Fact]
    public async Task CountByUser_ReturnsCorrectNumber()
    {
        _carServiceMock.Setup(s => s.GetByUserId("user1"))
            .ReturnsAsync(new List<CarDto> { new CarDto(), new CarDto() });

        var result = await _controller.CountByUser("user1");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(2, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_OnException()
    {
        var dto = new CarDto { LicencePlate = "ABC123" };
        _carServiceMock.Setup(s => s.Create(dto)).ThrowsAsync(new Exception("Duplicate"));

        var result = await _controller.Create(dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Duplicate", bad.Value);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_OnIdMismatch()
    {
        var dto = new CarDto { Id = 1 };
        var result = await _controller.Update(2, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var result = await _controller.Delete(7);

        _carServiceMock.Verify(s => s.Delete(7), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAllByUser_CallsDeleteForEach()
    {
        _carServiceMock.Setup(s => s.GetByUserId("user1"))
            .ReturnsAsync(new List<CarDto> {
                new CarDto { Id = 1 },
                new CarDto { Id = 2 }
            });

        var result = await _controller.DeleteAllByUser("user1");

        _carServiceMock.Verify(s => s.Delete(1), Times.Once);
        _carServiceMock.Verify(s => s.Delete(2), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }
}
