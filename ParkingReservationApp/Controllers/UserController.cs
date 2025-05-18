using Microsoft.AspNetCore.Mvc;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Services;

namespace ParkingReservationApp.Controllers;
/// <summary>
/// 
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound("User not found");
        }
        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
    {
        var result = await _userService.Register(registerDto);
        if (result == null)
        {
            return BadRequest("Please enter a valid email and password.");
        }
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
    {
     var result = await _userService.Login(loginDto);
     if (result == null)
     {
         return Unauthorized("Invalid credentials");
     }
     return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _userService.DeleteUser(id);
        return NoContent();
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateDto passwordUpdateDto)
    {
        var success = await _userService.UpdatePassword(passwordUpdateDto);
        if (!success)
        {
            return BadRequest("Invalid password");
        }
        return Ok("Password updated");
    }
}