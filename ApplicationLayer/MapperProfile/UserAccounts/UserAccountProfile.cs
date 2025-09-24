using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.DTOs.User;
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
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<UpdateUserProfileDto, UserProfile>()
            .ForMember(dest => dest.CountryOfResidenceId, opt => opt.MapFrom(src => src.CountryOfResidenceId))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

            CreateMap<TelegramUserInformationDto, TelegramUserInformation>();
            CreateMap<UserAccount, UserInvitedResultDto>()
                .ForMember(dest => dest.UserAccountId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                    src.UserProfiles.FirstOrDefault() != null
                        ? (src.UserProfiles.FirstOrDefault().DisplayName ??
                           src.UserProfiles.FirstOrDefault().FirstName + " " +
                           src.UserProfiles.FirstOrDefault().LastName) : string.Empty))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.UserProfiles.FirstOrDefault().Address))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.UserProfiles.FirstOrDefault().Company))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.PhoneNumber) && src.PhoneNumber.Length >= 7
                        ? src.PhoneNumber.Substring(0, 3) + "****" +
                          src.PhoneNumber.Substring(src.PhoneNumber.Length - 3)
                        : src.PhoneNumber
                ));
        }
    }
}