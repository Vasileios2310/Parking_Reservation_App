using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Data;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Repositories;

/// <summary>
/// 
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    ///  Repository implementation for managing application users.
    /// </summary>
    /// <param name="context"></param>
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
    {
       return await _context.Users.ToListAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<ApplicationUser?> GetUserByEmail(string email)
    {
       return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ApplicationUser?> GetUserByUsername(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    /// <summary>
    /// Finds and retrieves a collection of users assigned to a specific user role.
    /// </summary>
    /// <param name="userRole">The role to filter users by.</param>
    /// <returns>A collection of users associated with the specified role.</returns>
    public async Task<IEnumerable<ApplicationUser>> FindUsersByRole(UserRole userRole)
    {
        var roleName = userRole.GetDisplayName();
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
            return Enumerable.Empty<ApplicationUser>();
        }
        return await _context.Users
            .Where(u => _context.UserRoles
                .Any(ur => ur.RoleId == role.Id && ur.UserId == u.Id))
            .ToListAsync();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    public Task AddUser(ApplicationUser user)
    {
        _context.Users.Add(user);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    public void UpdateUser(ApplicationUser user)
    {
        if(user == null) throw new ArgumentNullException(nameof(user));
        _context.Users.Update(user);
    }

    /// <summary>
    /// Deletes a user by ID. Changes must be persisted with SaveChangesAsync.
    /// </summary>
    /// <param name="id"></param>
    public async Task DeleteUserAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }

    public async Task<bool> UserExists(string emailOrId)
    {
        if (string.IsNullOrWhiteSpace(emailOrId))
            return false;
        
        return await _context.Users.AnyAsync(u => u.Id == emailOrId || u.Email == emailOrId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<PaginatedResult<ApplicationUser>> GetUsersPagedAsync(int pageNumber, int pageSize)
    {
        if(pageNumber < 1 ) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if(pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        var query = _context.Users.AsNoTracking().OrderBy(u => int.Parse(u.Id));
        var totalCount = await query.CountAsync();
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PaginatedResult<ApplicationUser>(
                                                    items, 
                                                    pageNumber,
                                                    pageSize, 
                                                    totalCount);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task SaveChangesAsync() =>  _context.SaveChangesAsync();
}