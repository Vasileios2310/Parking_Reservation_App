using AutoMapper;
using ParkingReservationApp.Data;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;

namespace ParkingReservationApp.Services;

/// <summary>
/// Service responsible for managing parking reservations.
/// Provides methods to create, retrieve, delete, and manage reservations by various criteria such as user, car, parking space, or date range.
/// </summary>
public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public ReservationService(IReservationRepository reservationRepository, IMapper mapper , ApplicationDbContext context)
    {
        _reservationRepository = reservationRepository;
        _mapper = mapper;   
        _context = context;
    }

    public async Task<IEnumerable<ReservationDto>> GetAll()
    {
        var reservations = await _reservationRepository.GetAll();
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<ReservationDto?> GetById(int id)
    {
        var reservation = await _reservationRepository.GetById(id);
        return _mapper.Map<ReservationDto?>(reservation);
    }

    public async Task<IEnumerable<ReservationDto>> GetByUserId(string userId)
    {
        var reservations = await _reservationRepository.GetByUserId(userId);
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);   
    }

    public async Task<ReservationDto> Create(ReservationDto dto)
    {
        if(dto.StartTime >= dto.EndTime)
            throw new Exception("Start time must be before end time");
        
        var reservation = _mapper.Map<Reservation>(dto);
        
        await _reservationRepository.Add(reservation);
        await _reservationRepository.SaveChangesAsync();
        
        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task Delete(int id)
    {
        await _reservationRepository.Delete(id);
        await _reservationRepository.SaveChangesAsync();   
    }

    public async Task<IEnumerable<ReservationDto>> GetByCarId(int carId)
    {
      var all = await _reservationRepository.GetAll();
      var result = all.Where(r => r.CarId == carId);
      return _mapper.Map<IEnumerable<ReservationDto>>(result); 
    }

    public async Task<IEnumerable<ReservationDto>> GetByParkingSpaceId(int spaceId)
    {
        var all = await _reservationRepository.GetAll();
        var result = all.Where(r => r.ParkingSpaceId == spaceId);
        return _mapper.Map<IEnumerable<ReservationDto>>(result);
    }

    public async Task<IEnumerable<ReservationDto>> GetByDateRange(DateTime start, DateTime end)
    {
        var all = await _reservationRepository.GetAll();
        var result = all.Where(r => r.StartTime >= start && r.EndTime <= end);
        return _mapper.Map<IEnumerable<ReservationDto>>(result);
    }

    public async Task<IEnumerable<ReservationDto>> GetUpcomingByUserId(string userId)
    {
        var userReservation = await _reservationRepository.GetByUserId(userId);
        var upcoming = userReservation.Where(r => r.EndTime > DateTime.Now);
        return _mapper.Map<IEnumerable<ReservationDto>>(upcoming);
    }

    public async Task<bool> MarkAsPaid(int reservationId)
    {
       var res = await _reservationRepository.GetById(reservationId);
       if(res == null) return false;
       
       res.IsPaid = true;
       await _reservationRepository.SaveChangesAsync();
       return true;
    }
    
    public async Task<bool> PayForReservation(ReservationPaymentDto dto)
    {
        var reservation = await _reservationRepository.GetById(dto.ReservationId);
        if (reservation == null || reservation.IsPaid || reservation.IsCancelled)
            return false;

        // Simulated card validation
        if (dto.CreditCardNumber.Length < 12)
            throw new Exception("Invalid card");

        reservation.IsPaid = true;

        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Payment",
            Entity = "Reservation",
            Description = $"Paid reservation #{reservation.Id} with card ending in {dto.CreditCardNumber[^4..]}",
            UserId = reservation.UserId,
            Timestamp = DateTime.UtcNow
        });

        await _reservationRepository.SaveChangesAsync();
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> CancelReservation(int id)
    {
        var reservation = await _reservationRepository.GetById(id);
        if (reservation == null || reservation.IsCancelled)
            return false;

        if (DateTime.UtcNow >= reservation.StartTime)
            throw new Exception("Cannot cancel a reservation that already started.");

        reservation.IsCancelled = true;

        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Cancel",
            Entity = "Reservation",
            Description = $"Cancelled reservation #{reservation.Id}",
            UserId = reservation.UserId,
            Timestamp = DateTime.UtcNow
        });

        await _reservationRepository.SaveChangesAsync();
        await _context.SaveChangesAsync();

        return true;
    }
}