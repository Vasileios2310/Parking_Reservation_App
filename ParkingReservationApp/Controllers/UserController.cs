using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public UserController(IUserService userService , UserManager<ApplicationUser> userManager , IEmailService emailService)
    {
        _userService = userService;
        _userManager = userManager;
        _emailService = emailService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
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

    [Authorize(Roles = "Admin")]
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

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ? Ok("Email Confirmed") : BadRequest("Invalid token");
    }

    [HttpGet("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || (!await _userManager.IsEmailConfirmedAsync(user)))
            return BadRequest("Invalid email or not confirmed");
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://yourfrontend.com/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        
        await _emailService.SendEmailAsync(email, "Reset Password", $"Please reset your password by clicking <a href='{resetLink}'>here</a>.");
        
        return Ok("Email sent");
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null) return BadRequest("User not found.");

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return Ok("Password has been reset.");
    }

}