using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ParkingReservationApp.Data;

/// <summary>
/// A factory class used to create instances of <see cref="ApplicationDbContext"/> at design time.
/// Implements the <see cref="IDesignTimeDbContextFactory{TContext}"/> interface to enable tools like Entity Framework migrations
/// to generate and configure the database context outside of the application runtime.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder
            .UseSqlite("Data Source=/Users/vasiliskr/Desktop/WebApplication/ParkingDB.sqlite;");
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}