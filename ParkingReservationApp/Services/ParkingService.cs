using AutoMapper;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;
using ParkingReservationApp.Repositories;

namespace ParkingReservationApp.Services;
/// <summary>
/// 
/// </summary>
public class ParkingService : IParkingService
{
    private readonly IParkingReposiotry _parkingRepository;
    private readonly IMapper _mapper;
    
    public ParkingService(IParkingReposiotry parkingRepository, IMapper mapper)
    {
        _parkingRepository = parkingRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<ParkingDto>> GetAllParkings()
    {
        var parkings = await _parkingRepository.GetAll();
        return _mapper.Map<IEnumerable<ParkingDto>>(parkings);
    }

    public async Task<ParkingDto?> GetParkingById(int id)
    {
        var parking = await _parkingRepository.GetById(id);
        return _mapper.Map<ParkingDto?>(parking); 
    }

    public async Task<IEnumerable<ParkingDto>> GetParkingByArea(string area)
    {
        var parkings = await _parkingRepository.GetByArea(area);
        return _mapper.Map<IEnumerable<ParkingDto>>(parkings);
    }

    public async Task<ParkingDto> CreateParking(ParkingDto parkingDto)
    {
        var parking = _mapper.Map<Parking>(parkingDto);
        await _parkingRepository.Add(parking);
        await _parkingRepository.SaveChanges();
        return _mapper.Map<ParkingDto>(parking);
    }

    public async Task DeleteParking(int id)
    {
        await _parkingRepository.Delete(id);
        await _parkingRepository.SaveChanges();
    }
}