using ApplicationLayer.CQRS.MiniApp.Command;
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
            .ForMember(dest => dest.Attachments, opt => opt.Ignore());

        CreateMap<UpdateRequestDto, Request>()
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.RequestItemTypes, opt => opt.Ignore())
           .ForMember(dest => dest.Attachments, opt => opt.Ignore());

        CreateMap<DeliverableOriginLocationDto, RequestAvailableOrigin>();
        CreateMap<DeliverableDestinationLocationDto, RequestAvailableDestination>();

        CreateMap<MiniApp_CreateRequestCommand, Request>()
            // این‌ها optional هستند اگر بخوای مقادیر nullable یا لیست‌ها را تنظیم کنی
            .ForMember(dest => dest.OriginCityId, opt => opt.MapFrom(src => src.OriginCityId))
            .ForMember(dest => dest.DestinationCityId, opt => opt.MapFrom(src => src.DestinationCityId))
            .ForMember(dest => dest.MaxWeightKg, opt => opt.MapFrom(src => src.MaxWeightKg))
            .ForMember(dest => dest.MaxLengthCm, opt => opt.MapFrom(src => src.MaxLengthCm))
            .ForMember(dest => dest.MaxWidthCm, opt => opt.MapFrom(src => src.MaxWidthCm))
            .ForMember(dest => dest.MaxHeightCm, opt => opt.MapFrom(src => src.MaxHeightCm))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.RequestItemTypes, opt => opt.Ignore()) // اگر لازم داری باید جداگانه Map شود
            .ForMember(dest => dest.Attachments, opt => opt.Ignore())       // فایل‌ها را باید جداگانه تبدیل کنی
            .ForMember(dest => dest.UserAccountId, opt => opt.Ignore())     // معمولا از context میاد
            .ForMember(dest => dest.SuggestedPrice, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableOrigins, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableDestinations, opt => opt.Ignore())
            .ForMember(dest => dest.Suggestions, opt => opt.Ignore());
    }
}