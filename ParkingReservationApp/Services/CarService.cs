using System.Security.Claims;
using AutoMapper;
using ParkingReservationApp.Data;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;

namespace ParkingReservationApp.Services;

/// <summary>
/// 
/// </summary>
public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
               ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    
    public CarService(ICarRepository carRepository, IMapper mapper, ApplicationDbContext context , IHttpContextAccessor httpContextAccessor)
    {
        _carRepository = carRepository;
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IEnumerable<CarDto>> GetByUserId(string userId)
    {
       var cars = await _carRepository.GetByUserId(userId);
       return _mapper.Map<IEnumerable<CarDto>>(cars);
    }

    public async Task<CarDto?> GetById(int id)
    {
        var car = await _carRepository.GetById(id);
        var currentUserId = GetCurrentUserId();
        if (car == null || car.UserId != currentUserId)
            return null;
        
        return _mapper.Map<CarDto?>(car);
    }

    public async Task<CarDto?> GetByPlate(string plate)
    {
        var car = await _carRepository.GetByPlate(plate);
        return _mapper.Map<CarDto?>(car);
    }

    public async Task<CarDto> Create(CarDto dto)
    {
        var currentUserId = GetCurrentUserId();

        if (dto.UserId != currentUserId)
            throw new UnauthorizedAccessException("Invalid user assignment.");

        var existing = await _carRepository.GetByPlate(dto.LicencePlate);
        if (existing != null)
            throw new Exception("Car already exists.");

        var car = _mapper.Map<Car>(dto);
        await _carRepository.Add(car);
        await _carRepository.SaveChangesAsync();

        return _mapper.Map<CarDto>(car);
    }


    public async Task Delete(int id)
    {
        var car = await _carRepository.GetById(id);
        var currentUserId = GetCurrentUserId();

        if (car == null || car.UserId != currentUserId)
            throw new UnauthorizedAccessException("Not your car.");

        await _carRepository.Delete(id);
        await _carRepository.SaveChangesAsync();
    }

    
    public async Task<bool> PlateExists(string plate)
    {
        var car = await _carRepository.GetByPlate(plate);
        return car != null;
    }

    public async Task<int> CountByUser(string userId)
    {
        var cars = await _carRepository.GetByUserId(userId);
        return cars.Count();
    }

    public async Task<CarDto> Update(CarDto dto)
    {
        var existing = await _carRepository.GetById(dto.Id);
        var currentUserId = GetCurrentUserId();

        if (existing == null)
            throw new Exception("Car not found.");

        if (existing.UserId != currentUserId)
            throw new UnauthorizedAccessException("Access denied.");

        if (existing.LicencePlate != dto.LicencePlate)
        {
            var plateInUse = await _carRepository.GetByPlate(dto.LicencePlate);
            if (plateInUse != null)
                throw new Exception("Licence plate already exists.");
        }

        existing.LicencePlate = dto.LicencePlate;

        await _carRepository.SaveChangesAsync();
        return _mapper.Map<CarDto>(existing);
    }


    public async Task DeleteAllByUser(string userId)
    {
        var cars = await _carRepository.GetByUserId(userId);
        foreach (var car in cars)
        {
            await _carRepository.Delete(car.Id);
        }

        await _carRepository.SaveChangesAsync();
    }
    
    public async Task DeletePermanent(int id)
    {
        var car = await _carRepository.GetById(id);
        if (car == null) throw new Exception("Car not found.");

        await _carRepository.HardDelete(id);
        await _carRepository.SaveChangesAsync();

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? "unknown";

        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Delete",
            Entity = "Car",
            Description = $"Car ID {id} permanently deleted.",
            UserId = userId,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

}