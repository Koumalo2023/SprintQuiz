using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class CoursProfile : Profile
    {
        public CoursProfile()
        {
            // Mapping Cours → CoursDto
            CreateMap<Cours, CoursDto>()
                .ForMember(dest => dest.ModuleNom, opt => opt.MapFrom(src => src.Module.Nom))
                .ForMember(dest => dest.DerniereActivite, opt => opt.Ignore());

            // Mapping CreateCoursDto → Cours
            CreateMap<CreateCoursDto, Cours>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DerniereModification, opt => opt.MapFrom(src => (DateTime?)null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>()))
                .ForMember(dest => dest.EstActif, opt => opt.MapFrom(src => src.EstActif))
                .ForMember(dest => dest.Objectifs, opt => opt.MapFrom(src => src.Objectifs))
                .ForMember(dest => dest.Resume, opt => opt.MapFrom(src => src.Resume))
                .ForMember(dest => dest.NotionsCles, opt => opt.MapFrom(src => src.NotionsCles)) 
                .ForMember(dest => dest.DateOuverture, opt => opt.MapFrom(src => src.DateOuverture));

            // Mapping UpdateCoursDto → Cours 
            CreateMap<UpdateCoursDto, Cours>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore())
                .ForMember(dest => dest.DerniereModification, opt => opt.MapFrom(src => DateTime.UtcNow))

                .ForMember(dest => dest.Nom, opt => opt.Condition(src => src.Nom != null))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.Ordre, opt => opt.Condition(src => src.Ordre.HasValue))
                .ForMember(dest => dest.ModuleId, opt => opt.Condition(src => src.ModuleId.HasValue))

                .ForMember(dest => dest.EstActif, opt => opt.Condition(src => src.EstActif.HasValue))
                .ForMember(dest => dest.DateOuverture, opt => opt.Condition(src => src.DateOuverture.HasValue))
                .ForMember(dest => dest.Objectifs, opt => opt.Condition(src => src.Objectifs != null))
                .ForMember(dest => dest.Resume, opt => opt.Condition(src => src.Resume != null))
                .ForMember(dest => dest.NotionsCles, opt => opt.Condition(src => src.NotionsCles != null))
                .ForMember(dest => dest.Tags, opt => opt.Condition(src => src.Tags != null));
        }
    }
}

