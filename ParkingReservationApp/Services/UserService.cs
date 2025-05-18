using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;

namespace ParkingReservationApp.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, 
                        UserManager<ApplicationUser> userManager,
                        SignInManager<ApplicationUser> signInManager,
                        IMapper mapper)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsers();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserById(string id)
    {
        var user = await _userRepository.GetUserByUsername(id);
        return _mapper.Map<UserDto?>(user);
    }

    public async Task<UserDto> CreateUser(UserDto userDto)
    {
        var user = _mapper.Map<ApplicationUser>(userDto);
        await _userRepository.AddUser(user);
        await _userRepository.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUser(UserDto userDto)
    {
        var user = _mapper.Map<ApplicationUser>(userDto);
        _userRepository.UpdateUser(user);
        await _userRepository.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUser(string id)
    {
        await _userRepository.DeleteUserAsync(id);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<UserDto?> Login(LoginRequestDto loginRequestDto)
    {
        var loginUser = await _userManager.FindByEmailAsync(loginRequestDto.Email);
        if (loginUser == null)
        {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(loginUser , loginRequestDto.Password , false);
        return result.Succeeded ? _mapper.Map<UserDto>(loginUser) : null;
    }

    public async Task<UserDto> Register(RegisterRequestDto registerRequestDto)
    {
        if (registerRequestDto == null)
        {
            throw new ArgumentNullException(nameof(registerRequestDto));
        }

        var user = new ApplicationUser
        {
            Firstname = registerRequestDto.Firstname,
            Lastname = registerRequestDto.Lastname,
            Email = registerRequestDto.Email,
            UserName = registerRequestDto.Email
        };
        
        var result = await _userManager.CreateAsync(user , registerRequestDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Registration failed: {errors}");
        }
        
        user.Cars = registerRequestDto.LicencePlates
                .Select(lp => new Car {LicencePlate = lp , UserId = user.Id}).ToList();
        
        await _userRepository.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByEmail(string email)
    {
        var user = await _userRepository.GetUserByEmail(email);
        return _mapper.Map<UserDto?>(user);
    }

    public async Task<bool> UpdatePassword(PasswordUpdateDto passwordUpdateDto)
    {
        var user = await _userManager.FindByEmailAsync(passwordUpdateDto.Email);
        if (user == null || passwordUpdateDto.NewPassword != passwordUpdateDto.ConfirmPassword) return false;
        
        var result = await _userManager.ChangePasswordAsync(user , passwordUpdateDto.OldPassword , passwordUpdateDto.NewPassword);
        return result.Succeeded;
    }
}