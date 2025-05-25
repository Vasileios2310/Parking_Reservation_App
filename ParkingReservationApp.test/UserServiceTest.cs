using Xunit;
using Moq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ParkingReservationApp.Models;
using ParkingReservationApp.Services;
using ParkingReservationApp.Repositories;
using ParkingReservationApp.DTOs;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Assert = Xunit.Assert;

namespace ParkingReservationApp.test;
/// <summary>
/// 
/// </summary>
public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;
    private readonly Mock<IEmailService> _emailServiceMock;
    
    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        var storeMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            storeMock.Object, null, null, null, null, null, null, null, null);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object,
            contextAccessorMock.Object,
            claimsFactoryMock.Object,
            null, null, null, null);

        _mapperMock = new Mock<IMapper>();
        _emailServiceMock = new Mock<IEmailService>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _mapperMock.Object,
            _emailServiceMock.Object);
    }
    
    [Fact]
    public async Task Register_Should_Create_User_And_MapToDto()
    {
        // Arrange
        var registeredDto = new RegisterRequestDto
        {
            Firstname = "John",
            Lastname = "Doe",
                Email = "<example@example.com>",
                Password = "<P@ssw0rd!>",
                LicencePlates = new List<string> { "ABC123", "DEF456" }
            };
            // Create User
            var createdUser = new ApplicationUser
            {
                Id = "U1",
                Email = "example@example.com"
            };
            // Expected DTO
            var expectedDto = new UserDto
            {
                Id = "U1",
                Email = "example@example.com"
            };
            
            _userManagerMock.Setup(x=>x.CreateAsync(It.IsAny<ApplicationUser>(), registeredDto.Password))
                .ReturnsAsync(IdentityResult.Success);
            
            _mapperMock.Setup(m=>m.Map<UserDto>(It.IsAny<ApplicationUser>()))
                .Returns(expectedDto);
            
            // Act
            var result = await _userService.Register(registeredDto);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal("example@example.com" , result.Email );
            Assert.Equal("U1" , result.Id);
            
            _userManagerMock.Verify(x=>x.CreateAsync(It.IsAny<ApplicationUser>(), registeredDto.Password), Times.Once);
            _userRepositoryMock.Verify(x=>x.SaveChangesAsync(), Times.Once);
        }

    [Fact]
    public async Task Login_Should_Return_Null_If_Password_Wrong()
    {
        var loginDto = new LoginRequestDto
        {
            Email = "user@example.com",
            Password = "wrongpassword"
        };
        
        var fakeUser = new ApplicationUser
        {
            Email = "user@example.com",
            UserName = "test@example.com"
        };
        
        _userManagerMock.Setup(m => m.FindByEmailAsync(loginDto.Email)).ReturnsAsync(fakeUser);
        _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(fakeUser, loginDto.Password,false))
            .ReturnsAsync(SignInResult.Failed);
        
        var result = await _userService.Login(loginDto);
        Assert.Null(result);
        _userManagerMock.Verify(x=>x.FindByEmailAsync(loginDto.Email), Times.Once);
        _signInManagerMock.Verify(x=>x.CheckPasswordSignInAsync(fakeUser, loginDto.Password,false), Times.Once);
    }

    [Fact]
    public async Task Login_Should_Return_UserDto_If_Credentials_Valid()
    {
        var loginDto = new LoginRequestDto
        {
            Email = "valid@example.com",
            Password = "correctpassword"
        };
        
        var appUser = new ApplicationUser { Email = loginDto.Email };
        var userDto = new UserDto { Email = loginDto.Email };
        
        _userManagerMock.Setup(m => m.FindByEmailAsync(loginDto.Email)).ReturnsAsync(appUser);
        _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(appUser, loginDto.Password,false))
            .ReturnsAsync(SignInResult.Success);
        
        _mapperMock.Setup(m => m.Map<UserDto>(appUser)).Returns(userDto);
        
        var result = await _userService.Login(loginDto);
        Assert.NotNull(result);
        Assert.Equal(userDto.Email, result.Email);
    }

    [Fact]
    public async Task Register_Should_Create_User_And_SendConfirmationEmail()
    {
        // Arrange
        var registerDto = new RegisterRequestDto
        {
            Firstname = "John",
            Lastname = "Doe",
            Email = "example@example.com",
            Password = "P@ssw0rd!",
            LicencePlates = new List<string> { "ABC123", "DEF456" }
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token123");
        _emailServiceMock.Setup(x => x.SendEmailAsync(registerDto.Email, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()))
            .Returns(new UserDto { Id = "U1", Email = registerDto.Email });

        // Act
        var result = await _userService.Register(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("example@example.com", result.Email);
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password), Times.Once);
        _userManagerMock.Verify(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _emailServiceMock.Verify(x => x.SendEmailAsync(registerDto.Email, It.IsAny<string>(), It.Is<string>(body => body.Contains("confirm-email"))), Times.Once);
        //_userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
