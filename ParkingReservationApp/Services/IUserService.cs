using ParkingReservationApp.DTOs;

namespace ParkingReservationApp.Services;
/// <summary>
/// 
/// </summary>
public interface IUserService
{
    //CRUD
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(string id);
    Task<UserDto> CreateUser(UserDto userDto);
    Task<UserDto> UpdateUser(UserDto userDto);
    Task DeleteUser(string id);
    
    //AUTH
    Task<UserDto?> Login(LoginRequestDto loginRequestDto);
    Task<UserDto> Register(RegisterRequestDto registerRequestDto);
    
    //LOOKUP
    Task<UserDto?> GetByEmail(string email);
    
    //Password management
    Task<bool> UpdatePassword(PasswordUpdateDto passwordUpdateDto);
}