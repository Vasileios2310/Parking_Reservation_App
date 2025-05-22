using AutoMapper;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;

namespace ParkingReservationApp.Services;

/// <summary>
/// Provides services for managing parking spaces.
/// </summary>
/// <remarks>
/// This service provides functionality to perform CRUD (Create, Read, Delete)
/// operations for parking spaces. It interacts with the data repository to fetch,
/// create, or delete parking space records. Additionally, data transfer objects
/// (DTOs) are used to handle input and output between the service and its clients.
/// </remarks>
public class ParkingSpaceService : IParkingSpaceService
{
    
    private readonly IParkingSpaceRepository _repository;
    private readonly IMapper _mapper;

    public ParkingSpaceService(IParkingSpaceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ParkingSpaceDto>> GetAll()
    {
        var spaces = await _repository.GetAll();
        return _mapper.Map<IEnumerable<ParkingSpaceDto>>(spaces);
    }

    public async Task<IEnumerable<ParkingSpaceDto>> GetByParkingId(int parkingId)
    {
        var spaces = await _repository.GetByParkingId(parkingId);
        return _mapper.Map<IEnumerable<ParkingSpaceDto>>(spaces);
    }

    public async Task<ParkingSpaceDto?> GetById(int id)
    {
        var spaces = await _repository.GetById(id);
        return _mapper.Map<ParkingSpaceDto?>(spaces);
    }

    public async Task<ParkingSpaceDto> Create(ParkingSpaceDto dto)
    {
        var existing = await _repository.GetByParkingId(dto.ParkingId);
        if(existing.Any(s => s.SpaceNumber == dto.SpaceNumber))
            throw new Exception("Space number already exists in this parking");
        
        var space = _mapper.Map<ParkingSpace>(dto);
        await _repository.Add(space);
        return _mapper.Map<ParkingSpaceDto>(space);
    }

    public async Task Delete(int id)
    {
        await _repository.Delete(id);
        await _repository.SaveChangesAsync();
    }
}