using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ParkingReservationApp.Controllers;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Services;
using Assert = Xunit.Assert;

namespace ParkingReservationApp.test;

public class ReservationControllerTest
{
    private readonly Mock<IReservationService> _serviceMock;
    private readonly ReservationController _controller;

    public ReservationControllerTest()
    {
        _serviceMock = new Mock<IReservationService>();
        _controller = new ReservationController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithData()
    {
        var reservations = new List<ReservationDto>
        {
            new ReservationDto { Id = 1 },
            new ReservationDto { Id = 2 }
        };

        _serviceMock.Setup(s => s.GetAll()).ReturnsAsync(reservations);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<ReservationDto>>(okResult.Value);
        Assert.Equal(2, data.Count());
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNull()
    {
        _serviceMock.Setup(s => s.GetById(10)).ReturnsAsync((ReservationDto?)null);

        var result = await _controller.GetById(10);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var dto = new ReservationDto { Id = 5 };
        _serviceMock.Setup(s => s.GetById(5)).ReturnsAsync(dto);

        var result = await _controller.GetById(5);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenValid()
    {
        var dto = new ReservationDto { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
        _serviceMock.Setup(s => s.Create(dto)).ReturnsAsync(dto);

        var result = await _controller.Create(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_OnError()
    {
        var dto = new ReservationDto { StartTime = DateTime.Now.AddHours(2), EndTime = DateTime.Now };

        _serviceMock.Setup(s => s.Create(dto)).ThrowsAsync(new Exception("Invalid time"));

        var result = await _controller.Create(dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid time", bad.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var result = await _controller.Delete(7);
        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.Delete(7), Times.Once);
    }

    [Fact]
    public async Task MarkAsPaid_ReturnsOk_WhenSuccess()
    {
        _serviceMock.Setup(s => s.MarkAsPaid(1)).ReturnsAsync(true);

        var result = await _controller.MarkAsPaid(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Marked as paid.", ok.Value);
    }

    [Fact]
    public async Task MarkAsPaid_ReturnsNotFound_WhenFailed()
    {
        _serviceMock.Setup(s => s.MarkAsPaid(999)).ReturnsAsync(false);

        var result = await _controller.MarkAsPaid(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Reservation not found.", notFound.Value);
    }
}
