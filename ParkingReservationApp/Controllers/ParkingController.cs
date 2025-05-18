using Microsoft.AspNetCore.Mvc;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Services;

namespace ParkingReservationApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParkingController : ControllerBase
{
    private readonly IParkingService _parkingService;

    public ParkingController(IParkingService parkingService)
    {
        _parkingService = parkingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var parkings = await _parkingService.GetAllParkings();
        return Ok(parkings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var parking = await _parkingService.GetParkingById(id);
        if (parking == null)
        {
            return NotFound("Parking not found");
        }
        return Ok(parking);
    }

    [HttpGet("area/{area}")]
    public async Task<IActionResult> GetByArea(string area)
    {
        var parking = await _parkingService.GetParkingByArea(area);
        if (area == null)
        {
            return NotFound("Parking not found");
        }
        return Ok(parking);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ParkingDto parkingDto)
    {
        var parking = await _parkingService.CreateParking(parkingDto);
        return Ok(parking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _parkingService.DeleteParking(id);
        return NoContent();
    }
}