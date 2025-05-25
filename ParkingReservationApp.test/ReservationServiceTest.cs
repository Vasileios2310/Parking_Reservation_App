using Moq;
using Xunit;
using AutoMapper;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;
using ParkingReservationApp.Services;
using Assert = Xunit.Assert;

namespace ParkingReservationApp.test;

/// <summary>
/// Provides unit tests for the ReservationService class.
/// Includes various test cases to evaluate the correctness of the
/// business logic related to reservation operations such as fetching, creating,
/// updating, and validating data.
/// </summary>
public class ReservationServiceTest
{
    private readonly Mock<IReservationRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ReservationService _service;

    public ReservationServiceTest()
    {
        _repoMock = new Mock<IReservationRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new ReservationService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedDtos()
    {
        var reservations = new List<Reservation> { new Reservation { Id = 1 } };
        var dtos = new List<ReservationDto> { new ReservationDto { Id = 1 } };

        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(reservations);
        _mapperMock.Setup(m => m.Map<IEnumerable<ReservationDto>>(reservations)).Returns(dtos);

        var result = await _service.GetAll();

        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }

    [Fact]
    public async Task GetById_ReturnsDto_WhenExists()
    {
        var entity = new Reservation { Id = 5 };
        var dto = new ReservationDto { Id = 5 };

        _repoMock.Setup(r => r.GetById(5)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<ReservationDto?>(entity)).Returns(dto);

        var result = await _service.GetById(5);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
    }

    [Fact]
    public async Task Create_Throws_WhenTimeInvalid()
    {
        var dto = new ReservationDto
        {
            StartTime = DateTime.Now.AddHours(2),
            EndTime = DateTime.Now.AddHours(1)
        };

        await Assert.ThrowsAsync<Exception>(() => _service.Create(dto));
    }

    [Fact]
    public async Task Create_AddsReservation_WhenValid()
    {
        var dto = new ReservationDto
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2)
        };
        var entity = new Reservation { Id = 99 };

        _mapperMock.Setup(m => m.Map<Reservation>(dto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<ReservationDto>(entity)).Returns(new ReservationDto { Id = 99 });
        _repoMock.Setup(r => r.Add(entity)).Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.Create(dto);

        Assert.Equal(99, result.Id);
    }

    [Fact]
    public async Task MarkAsPaid_UpdatesReservation_WhenExists()
    {
        var reservation = new Reservation { Id = 1, IsPaid = false };
        _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(reservation);
        _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.MarkAsPaid(1);

        Assert.True(result);
        Assert.True(reservation.IsPaid);
    }

    [Fact]
    public async Task MarkAsPaid_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetById(999)).ReturnsAsync((Reservation?)null);

        var result = await _service.MarkAsPaid(999);

        Assert.False(result);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsExpected()
    {
        var now = DateTime.Now;
        var all = new List<Reservation>
        {
            new Reservation { Id = 1, StartTime = now, EndTime = now.AddHours(1) },
            new Reservation { Id = 2, StartTime = now.AddDays(2), EndTime = now.AddDays(2).AddHours(1) }
        };

        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(all);
        _mapperMock.Setup(m => m.Map<IEnumerable<ReservationDto>>(It.IsAny<IEnumerable<Reservation>>()))
            .Returns((IEnumerable<Reservation> input) => input.Select(r => new ReservationDto { Id = r.Id }));

        var result = await _service.GetByDateRange(now.AddMinutes(-1), now.AddDays(1));

        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }
}
