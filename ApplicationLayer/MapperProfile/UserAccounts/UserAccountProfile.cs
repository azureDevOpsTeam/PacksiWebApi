using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.Extensions;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.MapperProfile.UserAccounts
{
    public class UserAccountProfile : Profile
    {
        public UserAccountProfile()
        {
            CreateMap<SignUpDto, UserAccount>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => PhoneNumberHelper.NormalizePhoneNumber(src.PhonePrefix, src.PhoneNumber)));

            CreateMap<SignUpDto, UserProfile>();
            CreateMap<TelegramMiniAppUserDto, UserProfile>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<TelegramUserInformationDto, TelegramUserInformation>();
        }
    }
}