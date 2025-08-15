using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class ProfilProfile : Profile
    {
        public ProfilProfile()
        {
            CreateMap<Utilisateur, ProfilDto>()
                .ForMember(dest => dest.StatistiquesGlobales, opt => opt.MapFrom(src => src.StatistiquesGlobales))
                .ForMember(dest => dest.ProgressionGlobale, opt => opt.Ignore()) // Calculé dans le service
                .ForMember(dest => dest.DernieresActivites, opt => opt.Ignore()); // Calculé dans le service

            CreateMap<UpdateProfilDto, Utilisateur>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

