// Mappings/FormationProfile.cs
using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models; 

namespace SprintQuiz.Mappings
{
    public class FormationProfile : Profile
    {
        public FormationProfile()
        {
            // Formation → FormationDto
            CreateMap<Formation, FormationDto>()
                .ForMember(dest => dest.Sprints, opt => opt.MapFrom(src => src.Sprints))
                .ForMember(dest => dest.DerniereActivite, opt => opt.Ignore());

            // CreateFormationDto → Formation
            CreateMap<CreateFormationDto, Formation>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DerniereModification, opt => opt.MapFrom(src => (DateTime?)null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>()));

            // UpdateFormationDto → Formation
            CreateMap<UpdateFormationDto, Formation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore())
                .ForMember(dest => dest.DerniereModification, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Nom, opt => opt.Condition(src => src.Nom != null))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.Ordre, opt => opt.Condition(src => src.Ordre.HasValue))
                .ForMember(dest => dest.EstActif, opt => opt.Condition(src => src.EstActif.HasValue))
                .ForMember(dest => dest.DateOuverture, opt => opt.Condition(src => src.DateOuverture.HasValue))
                .ForMember(dest => dest.Objectifs, opt => opt.Condition(src => src.Objectifs != null))
                .ForMember(dest => dest.Resume, opt => opt.Condition(src => src.Resume != null))
                .ForMember(dest => dest.NotionsCles, opt => opt.Condition(src => src.NotionsCles != null)) 
                .ForMember(dest => dest.Tags, opt => opt.Condition(src => src.Tags != null));
        }
    }
}