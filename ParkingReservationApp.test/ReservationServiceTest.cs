using Moq;
using Xunit;
using AutoMapper;
using ParkingReservationApp.Data;
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
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ReservationService _service;
    

    public ReservationServiceTest()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<ApplicationDbContext>();
        _service = new ReservationService(_reservationRepoMock.Object, _mapperMock.Object , _contextMock.Object);;
        
    }

    [Fact]
    public async Task GetAll_ReturnsMappedDtos()
    {
        var reservations = new List<Reservation> { new Reservation { Id = 1 } };
        var dtos = new List<ReservationDto> { new ReservationDto { Id = 1 } };

        _reservationRepoMock.Setup(r => r.GetAll()).ReturnsAsync(reservations);
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

        _reservationRepoMock.Setup(r => r.GetById(5)).ReturnsAsync(entity);
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
        _reservationRepoMock.Setup(r => r.Add(entity)).Returns(Task.CompletedTask);
        _reservationRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.Create(dto);

        Assert.Equal(99, result.Id);
    }

    [Fact]
    public async Task MarkAsPaid_UpdatesReservation_WhenExists()
    {
        var reservation = new Reservation { Id = 1, IsPaid = false };
        _reservationRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(reservation);
        _reservationRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.MarkAsPaid(1);

        Assert.True(result);
        Assert.True(reservation.IsPaid);
    }

    [Fact]
    public async Task MarkAsPaid_ReturnsFalse_WhenNotFound()
    {
        _reservationRepoMock.Setup(r => r.GetById(999)).ReturnsAsync((Reservation?)null);

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

        _reservationRepoMock.Setup(r => r.GetAll()).ReturnsAsync(all);
        _mapperMock.Setup(m => m.Map<IEnumerable<ReservationDto>>(It.IsAny<IEnumerable<Reservation>>()))
            .Returns((IEnumerable<Reservation> input) => input.Select(r => new ReservationDto { Id = r.Id }));

        var result = await _service.GetByDateRange(now.AddMinutes(-1), now.AddDays(1));

        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }
    
    [Fact]
    public async Task PayForReservation_Succeeds_WhenValid()
    {
        var reservation = new Reservation
        {
            Id = 1,
            IsPaid = false,
            IsCancelled = false,
            UserId = "user1"
        };

        _reservationRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(reservation);
        _reservationRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _service.PayForReservation(new ReservationPaymentDto
        {
            ReservationId = 1,
            CreditCardNumber = "4111111111111111",
            CardHolderName = "Test User",
            Expiration = "12/26",
            CVC = "123"
        });

        Assert.True(result);
        Assert.True(reservation.IsPaid);
    }

    [Fact]
    public async Task PayForReservation_Throws_OnInvalidCard()
    {
        var reservation = new Reservation { Id = 2, IsPaid = false };
        _reservationRepoMock.Setup(r => r.GetById(2)).ReturnsAsync(reservation);

        await Assert.ThrowsAsync<Exception>(() =>
            _service.PayForReservation(new ReservationPaymentDto
            {
                ReservationId = 2,
                CreditCardNumber = "123", // too short
            }));
    }

    [Fact]
    public async Task PayForReservation_Fails_IfAlreadyPaidOrCancelled()
    {
        _reservationRepoMock.Setup(r => r.GetById(3)).ReturnsAsync(new Reservation
        {
            Id = 3,
            IsPaid = true,
            IsCancelled = false
        });

        var result = await _service.PayForReservation(new ReservationPaymentDto
        {
            ReservationId = 3,
            CreditCardNumber = "4111111111111111"
        });

        Assert.False(result);
    }
    
    [Fact]
    public async Task CancelReservation_Succeeds_WhenValid()
    {
        var future = DateTime.UtcNow.AddHours(2);
        var reservation = new Reservation { Id = 10, StartTime = future, IsCancelled = false, UserId = "user1" };

        _reservationRepoMock.Setup(r => r.GetById(10)).ReturnsAsync(reservation);
        _reservationRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _service.CancelReservation(10);

        Assert.True(result);
        Assert.True(reservation.IsCancelled);
    }

    [Fact]
    public async Task CancelReservation_Fails_IfStarted()
    {
        var past = DateTime.UtcNow.AddMinutes(-5);
        var reservation = new Reservation { Id = 20, StartTime = past };

        _reservationRepoMock.Setup(r => r.GetById(20)).ReturnsAsync(reservation);

        await Assert.ThrowsAsync<Exception>(() => _service.CancelReservation(20));
    }

    [Fact]
    public async Task CancelReservation_Fails_IfAlreadyCancelled()
    {
        var reservation = new Reservation { Id = 30, StartTime = DateTime.UtcNow.AddMinutes(30), IsCancelled = true };
        _reservationRepoMock.Setup(r => r.GetById(30)).ReturnsAsync(reservation);

        var result = await _service.CancelReservation(30);

        Assert.False(result);
    }
}