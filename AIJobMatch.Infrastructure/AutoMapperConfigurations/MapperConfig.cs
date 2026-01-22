using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AIJobMatch.Domain.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.AutoMapperConfigurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<RegisterRequest, Account>().ReverseMap();
            
            CreateMap<CompanyRegisterRequest, Company>()
                .ForMember(dest => dest.VerificationStatus, opt => opt.MapFrom(src => VerificationStatus.Pending))
                .ForMember(dest => dest.VerifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RejectionReason, opt => opt.Ignore())
                .ForMember(dest => dest.Recruiters, opt => opt.Ignore());
            
            // Company -> CompanyRegisterResponse: Chuyển đổi dữ liệu từ entity sang response để trả về cho client
            CreateMap<Company, CompanyRegisterResponse>();
            
            // SubscriptionPlans Mapping
            CreateMap<SubscriptionPlanRequest, SubscriptionPlans>()
                .ForMember(dest => dest.UserSubscriptions, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());
            
            CreateMap<SubscriptionPlans, SubscriptionPlanResponse>();
        }
    }
}
