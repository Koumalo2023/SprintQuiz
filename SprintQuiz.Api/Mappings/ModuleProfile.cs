using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module, ModuleDto>()
                .ForMember(dest => dest.SprintNom, opt => opt.MapFrom(src => src.Sprint.Nom))
                .ForMember(dest => dest.Cours, opt => opt.MapFrom(src => src.Cours));

            CreateMap<CreateModuleDto, Module>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateModuleDto, Module>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

