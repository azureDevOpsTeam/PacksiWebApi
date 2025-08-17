using ApplicationLayer.DTOs.Advertisements;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.MapperProfile.Advertisements;

public class AdvertisementProfile : Profile
{
    public AdvertisementProfile()
    {
        CreateMap<AdScheduleDto, AdSchedule>();

        CreateMap<CreateAdvertisementDto, Advertisement>()
            .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules));
    }
}