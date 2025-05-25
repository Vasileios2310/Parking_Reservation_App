using ParkingReservationApp.DTOs;

namespace ParkingReservationApp.Services;

/// <summary>
/// Defines the operations for managing parking space reservations.
/// </summary>
public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAll();
    Task<ReservationDto?> GetById(int id);
    Task<IEnumerable<ReservationDto>> GetByUserId(string userId);
    Task<ReservationDto> Create(ReservationDto dto);
    Task Delete(int id);
    
    Task<IEnumerable<ReservationDto>> GetByCarId(int carId);
    Task<IEnumerable<ReservationDto>> GetByParkingSpaceId(int spaceId);
    Task<IEnumerable<ReservationDto>> GetByDateRange(DateTime start, DateTime end);
    Task<IEnumerable<ReservationDto>> GetUpcomingByUserId(string userId);
    Task<bool> MarkAsPaid(int reservationId);
    
    Task<bool> PayForReservation(ReservationPaymentDto paymentDto);
    
    Task<bool> CancelReservation(int id);


}