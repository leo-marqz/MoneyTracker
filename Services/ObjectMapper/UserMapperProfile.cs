
using AutoMapper;
using MoneyTracker.DTOs;
using MoneyTracker.Models;

namespace MoneyTracker.Services.ObjectMapper
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserRegistrationRequest, User>()
                .ForMember(
                    (dest) => dest.PasswordHash,
                    (opt) => opt.MapFrom((src => src.Password))
                )
                .ForMember(
                    (dest) => dest.UserName,
                    (opt) => opt.MapFrom((src) => src.Email)
                );
        }
    }
}