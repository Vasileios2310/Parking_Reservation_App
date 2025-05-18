using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Data;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;
using Xunit;
using Assert = Xunit.Assert;


namespace ParkingReservationApp.test;
/// <summary>
/// Test class implements IDisposable so we can clean up the in-memory context after each test
/// </summary>

public class UserRepositoryTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTest()
    {
        // Create new in-memory database with unique name (to isolate tests)
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;
        
        _context = new ApplicationDbContext(options);
        _userRepository = new UserRepository(_context);
    }

    // Dispose is called after each test finishes
    public void Dispose() => _context.Dispose();

    private void SeedUsers(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            // Create a new ApplicationUser with predictable Id, names, and email
            _context.Users.Add(new ApplicationUser
            {
                Id = i.ToString(),
                Firstname = $"User{i}",
                Lastname =  $"Last{i}",
                Email = $"user{i}@example.com"
            });
        }
        // Persist seed data immediately
        _context.SaveChanges();
    }

    /// <summary>
    /// Arrange: insert 2 users into the in-memory DB
    /// Act: call the repository method
    /// Assert: verify we got exactly 2 users back
    /// </summary>
    [Fact]
    public async Task GetAllUsers_ReturnsAllUsers()
    {
        SeedUsers(2);
        var users = await _userRepository.GetAllUsers();
        Assert.Equal(2, users.Count());
    }

    /// <summary>
    /// Arrange: add a single user with known email
    /// Act: fetch user by email
    /// Assert: user should exist
    /// </summary>
    [Fact]
    public async Task GetUserByEmail_ReturnsCorrectUser()
    {
        var u = new ApplicationUser
        {
            Id = "AA123",
            Firstname = "John",
            Lastname = "Doe",
            Email = "john111@example.com"
        };
        _context.Users.Add(u);
        await _context.SaveChangesAsync();

        var fetched = await _userRepository.GetUserByEmail("john111@example.com");
        Assert.NotNull(fetched);
        Assert.Equal(u.Id, fetched!.Id);
        Assert.Equal(u.Firstname, fetched.Firstname);
        Assert.Equal(u.Lastname, fetched.Lastname);
        Assert.Equal(u.Email, fetched.Email);
    }

    /// <summary>
    /// Arrange: add a single user with known email
    /// </summary>
    [Fact]
    public async Task GetUserByUsername_ReturnCorrectUser()
    {
        var u = new ApplicationUser
        {
            Id = "BB123",
            Firstname = "Alice",
            Lastname = "W.",
            Email = "alice@example.com"
        };
        _context.Users.Add(u);
        await _context.SaveChangesAsync();
        
        var fetched = await _userRepository.GetUserByUsername("BB123");
        Assert.NotNull(fetched);
        Assert.Equal(u.Id, fetched!.Id);
        Assert.Equal(u.Firstname, fetched.Firstname);
        Assert.Equal(u.Lastname, fetched.Lastname);
        Assert.Equal(u.Email, fetched.Email);
    }

    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task AddUser_SavesNewUser()
    {
        var u = new ApplicationUser
        {
            Id = "CC123",
            Firstname = "Bob",
            Lastname = "M.",
            Email = "Bob@example.com"
        };
        _userRepository.AddUser(u);
        await _userRepository.SaveChangesAsync();
        
        var fetched = await _userRepository.GetUserByUsername("CC123");
        Assert.NotNull(fetched);
        Assert.Equal("Bob" , fetched.Firstname);;
    }

    [Fact]
    public async Task UpdateUser_PersistsChanges()
    {
        var u = new ApplicationUser
        {
            Id = "DD123",
            Firstname = "Charlie",
            Lastname = "C.",
            Email = "charlie@example.com"
        };
        _context.Users.Add(u);
        await _context.SaveChangesAsync();
        
        u.Firstname = "Charlie222";
        _userRepository.UpdateUser(u);
        await _context.SaveChangesAsync();
        
        var fetched = await _userRepository.GetUserByUsername("DD123");
        Assert.NotNull(fetched);
        Assert.Equal("Charlie222" , fetched.Firstname);;
    }
    
    [Fact]
    public async Task DeleteUser_DeletesUser()
    {
        var u = new ApplicationUser
        {
            Id = "EE123",
            Firstname = "Danny",
            Lastname = "D.",
            Email = "danny@example.com"
        };
        _context.Users.Add(u);
        await _context.SaveChangesAsync();
        
        await _userRepository.DeleteUserAsync("EE123");
        await _context.SaveChangesAsync();
        
        var fetched = await _userRepository.GetUserByUsername("EE123");
        Assert.Null(fetched);
    }
    
    [Xunit.Theory]
    [InlineData("user1@example.com" , true)]
    [InlineData("1" , true)]
    [InlineData("noone@example.com" , false)]
    public async Task UserExists_ReturnsExpected(string arg, bool expected)
    {
        var u = new ApplicationUser
        {
            Id = "1",
            Firstname = "A.",
            Lastname = "D.",
            Email = "user1@example.com"
        };
        _context.Users.Add(u);
        await _context.SaveChangesAsync();
        
        var result = await _userRepository.UserExists(arg);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetUsersPagedAsync_FirstPage_Correct()
    {
        SeedUsers(25);
        
        var result = await _userRepository.GetUsersPagedAsync(1, 10);
        
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        
        Assert.Equal(10, result.Items.Count());
        
        Assert.Contains(result.Items, u => u.Email == "user1@example.com");
        Assert.DoesNotContain(result.Items , u => u.Email == "user111@example.com");
    }

    [Fact]
    public async Task GetUsersPagedAsync_LastPage_Correct()
    {
        SeedUsers(25);
        
        var result = await _userRepository.GetUsersPagedAsync(3, 10);
        
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        
        Assert.Equal(5, result.Items.Count());
        
        Assert.DoesNotContain(result.Items , u=> u.Email == "user11@example.com");
        Assert.Contains(result.Items , u=> u.Email == "user25@example.com");
        Assert.DoesNotContain(result.Items , u => u.Id == "11");
    }

    [Xunit.Theory]
    [InlineData(0, 5)]
    [InlineData(1, 0)]
    public async Task GetUsersPagedAsync_InvalidArgs_Throws(int pageNumber, int pageSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _userRepository.GetUsersPagedAsync(pageNumber, pageSize));
    }
}