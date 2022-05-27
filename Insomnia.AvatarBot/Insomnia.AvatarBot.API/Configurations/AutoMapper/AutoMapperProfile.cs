using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Insomnia.AvatarBot.Data;
using Insomnia.AvatarBot.Data.Entity;
using System;
using System.Linq;

namespace Insomnia.AvatarBot.API.Configurations.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // CreateMap<ClassDTO, ClassEntity>();
        }
    }
}
