using AutoMapper;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Application.Features.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, AppUserDto>().ReverseMap();
            CreateMap<RegisterRequestDto, AppUser>().ReverseMap();
            CreateMap<UpdateUserRequestDto, AppUser>().ReverseMap();
        }
    }
}
