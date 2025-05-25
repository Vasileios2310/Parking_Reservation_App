using Microsoft.AspNetCore.Mvc;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Services;

namespace ParkingReservationApp.Controllers;

/// <summary>
/// Handles reservation-related operations, including creating, retrieving, updating, and deleting reservations.
/// </summary>
/// <remarks>
/// This controller provides endpoints for managing reservations in the system. It interacts with the IReservationService
/// to perform operations such as retrieving all reservations, fetching reservation details by user, car, or parking space,
/// retrieving reservations within a specific date range, and marking reservations as paid.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
  private  readonly IReservationService _reservationService;

  public ReservationController(IReservationService reservationService)
  {
    _reservationService = reservationService;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var reservations = _reservationService.GetAll();
    return Ok(reservations);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var reservation = await _reservationService.GetById(id);
    if (reservation == null)
    {
      return NotFound("Reservation not found");
    }
    return Ok(reservation);
  }

  [HttpGet("{user/{userId}")]
  public async Task<IActionResult> GetByUser(string userId)
  {
    var reservation = _reservationService.GetByUserId(userId);
    return Ok(reservation);
  }

  [HttpGet("active/user/{userId}")]
  public async Task<IActionResult> GetUpcomingForUser(string userId) => Ok(await _reservationService.GetUpcomingByUserId(userId));
  
  [HttpGet("car/{carId}")]
  public async Task<IActionResult> GetByCar(int carId) => Ok(await _reservationService.GetByCarId(carId));
  
  [HttpGet("space/{spaceId}")]
  public async Task<IActionResult> GetByParkingSpace(int spaceId) => Ok(await _reservationService.GetByParkingSpaceId(spaceId));

  [HttpGet("range")]
  public async Task<IActionResult> GetByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end) => Ok(await _reservationService.GetByDateRange(start, end));
  
  [HttpPost]
  public async Task<IActionResult> Create([FromBody] ReservationDto reservationDto)
  {
    if(!ModelState.IsValid) return BadRequest(ModelState);

    try
    {
      var created = await _reservationService.Create(reservationDto);
      return Ok(created);
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpPut("{id}/pay")]
  public async Task<IActionResult> MarkAsPaid(int id)
  {
    var updated = await _reservationService.MarkAsPaid(id);
    if (!updated)
    {
      return NotFound("Reservation not found");
    }
    return Ok("Reservation marked as paid");
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _reservationService.Delete(id);
    return NoContent();
  }
}