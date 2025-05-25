using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Services;

namespace ParkingReservationApp.Controllers;

/// <summary>
/// Provides API endpoints for managing parking spaces.
/// </summary>
/// <remarks>
/// This controller handles operations such as retrieving all parking spaces,
/// retrieving parking spaces by parking lot ID, retrieving a parking space by its unique ID,
/// creating a new parking space, and deleting an existing parking space.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ParkingSpaceController : ControllerBase
{
    private readonly IParkingSpaceService _parkingSpaceService;
    
    public ParkingSpaceController(IParkingSpaceService parkingSpaceService)
    {
        _parkingSpaceService = parkingSpaceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var spaces = await _parkingSpaceService.GetAll();
        return Ok(spaces);
    }
    
    [HttpGet("parking/{parkingId}")]
    public async Task<IActionResult> GetByParking(int parkingId)
    {
        var spaces = await _parkingSpaceService.GetByParkingId(parkingId);
        return Ok(spaces);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var space = await _parkingSpaceService.GetById(id);
        if (space == null)
        {
            return NotFound("Space not found");
        }
        return Ok(space);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ParkingSpaceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var result = await _parkingSpaceService.Create(dto);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _parkingSpaceService.Delete(id);
        return NoContent();
    }
}