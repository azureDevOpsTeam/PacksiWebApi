using ApplicationLayer.DTOs.Commissions;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.MapperProfile.Commissions
{
    public class CommissionProfile : Profile
    {
        public CommissionProfile()
        {
            CreateMap<CreateCommissionDto, Commission>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.RequestPrice * 0.1m)) // فرض بر درصد ثابت
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}