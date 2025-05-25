using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Services;

namespace ParkingReservationApp.Controllers;

/// <summary>
/// 
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var cars = await _carService.GetByUserId(userId);
        return Ok(cars);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var car = await _carService.GetById(id);
        if (car == null) return NotFound();
        return Ok(car);
    }

    [HttpGet("plate/{plate}")]
    public async Task<IActionResult> GetByPlate(string plate)
    {
        var car = await _carService.GetByPlate(plate);
        if (car == null) return NotFound();
        return Ok(car);
    }

    [HttpGet("check-plate/{plate}")]
    public async Task<IActionResult> PlateExists(string plate)
    {
        var exists = await _carService.PlateExists(plate);
        return Ok(exists);
    }

    [HttpGet("user/{userId}/count")]
    public async Task<IActionResult> CountByUser(string userId)
    {
        var cars = await _carService.GetByUserId(userId);
        return Ok(cars.Count());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarDto dto)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var created  = await _carService.Create(dto);
            return Ok(created);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CarDto dto)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);
        if (id != dto.Id) return BadRequest("ID mismatch");

        try
        {
            var existing = await _carService.GetById(id);
            if(existing == null) return NotFound();

            await _carService.Delete(id);
            var updated = await _carService.Update(dto);
            
            return Ok(updated);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await _carService.GetById(id);
        if(car == null) return NotFound();
        
        var userId = User.FindFirst("sub")?.Value;
        if(car.UserId != userId) 
            return Forbid();
        
        await _carService.Delete(id);
        return NoContent();
    }
    
    [HttpDelete("user/{userId}/all")]
    public async Task<IActionResult> DeleteAllByUser(string userId)
    {
        var cars = await _carService.GetByUserId(userId);
        foreach (var car in cars)
        {
            await _carService.Delete(car.Id);
        }
        return NoContent();
    }
    
    /// <summary>
    /// This ensures only users with the "Admin" role can access the endpoint.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("hard/{id}")]
    public async Task<IActionResult> HardDelete(int id)
    {
        await _carService.DeletePermanent(id);
        return NoContent();
    }
    
    
}