using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class ExerciceProfile : Profile
    {
        public ExerciceProfile()
        {
            // Exercice
            CreateMap<Exercice, ExerciceDto>();
            CreateMap<CreateExerciceDto, Exercice>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateExerciceDto, Exercice>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Consultation Exercice
            CreateMap<ConsultationExercice, ConsultationExerciceDto>();
            CreateMap<CreateConsultationExerciceDto, ConsultationExercice>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateConsultation, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Indice
            CreateMap<Indice, IndiceDto>();
            CreateMap<CreateIndiceDto, Indice>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            // EtapeResolution
            CreateMap<EtapeResolution, EtapeResolutionDto>();
            CreateMap<CreateEtapeResolutionDto, EtapeResolution>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }
}
