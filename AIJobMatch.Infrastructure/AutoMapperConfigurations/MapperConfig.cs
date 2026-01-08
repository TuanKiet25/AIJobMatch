using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Entities;
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
        }
    }
}
