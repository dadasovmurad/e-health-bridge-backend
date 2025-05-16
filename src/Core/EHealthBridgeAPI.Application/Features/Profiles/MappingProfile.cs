using AutoMapper;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Application.Features.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Məsələn:
            CreateMap<AppUser, AppUserDto>().ReverseMap();
        }
    }
}
