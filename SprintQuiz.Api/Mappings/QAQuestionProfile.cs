using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class QAQuestionProfile : Profile
    {
        public QAQuestionProfile()
        {
            // QAQuestion
            CreateMap<QAQuestion, QAQuestionDto>()
                .ForMember(dest => dest.DureeEstimee, opt => opt.MapFrom(src => src.DureeEstimee))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
                .ForMember(dest => dest.DerniereActivite, opt => opt.Ignore());

            CreateMap<CreateQAQuestionDto, QAQuestion>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>()))
                .ForMember(dest => dest.DureeEstimee, opt => opt.Ignore());

            CreateMap<UpdateQAQuestionDto, QAQuestion>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore())
                .ForMember(dest => dest.DerniereModification, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Question, opt => opt.Condition(src => src.Question != null))
                .ForMember(dest => dest.Reponse, opt => opt.Condition(src => src.Reponse != null))
                .ForMember(dest => dest.Niveau, opt => opt.Condition(src => src.Niveau.HasValue))
                .ForMember(dest => dest.NiveauId, opt => opt.Condition(src => src.NiveauId.HasValue))
                .ForMember(dest => dest.NiveauDifficulte, opt => opt.Condition(src => src.NiveauDifficulte.HasValue))
                .ForMember(dest => dest.Tags, opt => opt.Condition(src => src.Tags != null))
                .ForMember(dest => dest.DureeEstimee, opt => opt.Ignore());

            // Consultation QA
            CreateMap<ConsultationQA, ConsultationQADto>();

            CreateMap<CreateConsultationQADto, ConsultationQA>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateConsultation, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
