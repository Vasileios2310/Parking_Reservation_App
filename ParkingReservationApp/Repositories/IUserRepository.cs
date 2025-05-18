using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;
/// <summary>
/// Repository interface for managing application users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    Task<IEnumerable<ApplicationUser>> GetAllUsers();

    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    Task<ApplicationUser?> GetUserByEmail(string email);
    
    /// <summary>
    /// Finds a user by their id.
    /// </summary>
    Task<ApplicationUser?> GetUserByUsername(string id);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    Task<IEnumerable<ApplicationUser>> FindUsersByRole(UserRole role);
    
    /// <summary>
    /// Add a new user.
    /// </summary>
    Task AddUser(ApplicationUser user);
    
    /// <summary>
    /// Updates an existing user.
    /// </summary>
    void UpdateUser(ApplicationUser user);

    /// <summary>
    /// Deletes a user by ID.
    /// </summary>
    Task DeleteUserAsync(string id);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailOrId"></param>
    /// <returns></returns>
    Task<bool> UserExists(string emailOrId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<PaginatedResult<ApplicationUser>> GetUsersPagedAsync(int pageNumber, int pageSize);
    
    /// <summary>
    /// Persists changes to the data store
    /// </summary>
    Task SaveChangesAsync();
}