using AutoMapper;
using TriageSystem.Models;
using TriageSystem.ViewModels;

namespace TriageSystem.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, LoginViewModel>();
            CreateMap<LoginViewModel, User>();
            CreateMap<User, RegisterViewModel>();
            CreateMap<RegisterViewModel, User>();

        }
    }
}