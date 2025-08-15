using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class QuizProfile : Profile
    {
        public QuizProfile()
        {
            CreateMap<Quiz, QuizDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));

            CreateMap<CreateQuizDto, Quiz>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateQuizDto, Quiz>()
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<QCMQuestion, QCMQuestionDto>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));

            CreateMap<CreateQCMQuestionDto, QCMQuestion>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateQCMQuestionDto, QCMQuestion>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<QCMOption, QCMOptionDto>();

            CreateMap<CreateQCMOptionDto, QCMOption>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateQCMOptionDto, QCMOption>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TentativeQuiz, TentativeQuizDto>()
                .ForMember(dest => dest.QuizTitre, opt => opt.MapFrom(src => src.Quiz.Titre));

            CreateMap<CreateTentativeQuizDto, TentativeQuiz>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.UtcNow));

            

            // Mapping Question
            CreateMap<QCMQuestion, QCMQuestionDto>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));

            CreateMap<CreateQCMQuestionDto, QCMQuestion>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateQCMQuestionDto, QCMQuestion>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Mapping Option
            CreateMap<QCMOption, QCMOptionDto>();

            CreateMap<CreateQCMOptionDto, QCMOption>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateQCMOptionDto, QCMOption>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}

