using AutoMapper;
using ParkingReservationApp.DTOs;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<Car, CarDto>();
        CreateMap<Parking, ParkingDto>();
        CreateMap<ParkingSpace, ParkingSpaceDto>();
        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.LicencePlate,
                opt => opt.MapFrom(src => src.Car.LicencePlate))
            .ForMember(des => des.ParkingName,
                opt => opt.MapFrom(src => src.ParkingSpace.Parking.Name));
            
        CreateMap<UserDto, ApplicationUser>();
        CreateMap<CarDto, Car>();
        CreateMap<ParkingDto, Parking>();
        CreateMap<ParkingSpaceDto, ParkingSpace>();
    }
}