using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkingReservationApp.Background;
using ParkingReservationApp.Data;
using ParkingReservationApp.Mapper;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;
using ParkingReservationApp.Services;

namespace ParkingReservationApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
        
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
        
        builder.Services.AddAuthorization();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IParkingService, ParkingService>();
        builder.Services.AddScoped<IParkingRepository, ParkingRepository>();
        builder.Services.AddScoped<IParkingSpaceRepository, ParkingSpaceRepository>();
        builder.Services.AddScoped<IParkingSpaceService, ParkingSpaceService>();
        builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
        builder.Services.AddScoped<IReservationService, ReservationService>();
        builder.Services.AddScoped<ICarRepository, CarRepository>();
        builder.Services.AddScoped<ICarService, CarService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddHostedService<ReservationNotifierService>();
        builder.Services.AddScoped<IEmailService, EmailService>(); // or mock/stub


        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 2;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.SignIn.RequireConfirmedEmail = true;
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseAuthentication();
        
        app.Run();
        
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await AppInitializer.SeedRolesAndAdmin(services);
        }
    }
}