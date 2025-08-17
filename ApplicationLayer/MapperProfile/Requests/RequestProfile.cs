using ApplicationLayer.DTOs.Requests;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.MapperProfile.Requests;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<CreateRequestDto, Request>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RequestItemTypes, opt => opt.Ignore())
            .ForMember(dest => dest.Attachments, opt => opt.Ignore())
            .ForMember(dest => dest.StatusHistories, opt => opt.Ignore());

        CreateMap<UpdateRequestDto, Request>()
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.RequestItemTypes, opt => opt.Ignore())
           .ForMember(dest => dest.Attachments, opt => opt.Ignore())
           .ForMember(dest => dest.StatusHistories, opt => opt.Ignore());

        CreateMap<DeliverableOriginLocationDto, RequestAvailableOrigin>();
        CreateMap<DeliverableDestinationLocationDto, RequestAvailableDestination>();
    }
}