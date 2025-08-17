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
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions))
            .ForMember(dest => dest.DureeEstimee, opt => opt.MapFrom(src => src.DureeEstimee))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.DerniereActivite, opt => opt.Ignore());

            // Mapping CreateQuizDto → Quiz
            CreateMap<CreateQuizDto, Quiz>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DerniereModification, opt => opt.MapFrom(src => (DateTime?)null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<string>()))
                .ForMember(dest => dest.DureeEstimee, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.Tentatives, opt => opt.Ignore());


            // Mapping UpdateQuizDto → Quiz
            CreateMap<UpdateQuizDto, Quiz>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore())
                .ForMember(dest => dest.DerniereModification, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Titre, opt => opt.Condition(src => src.Titre != null))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.Niveau, opt => opt.Condition(src => src.Niveau != null))
                .ForMember(dest => dest.NiveauId, opt => opt.Condition(src => src.NiveauId != null))
                .ForMember(dest => dest.Tags, opt => opt.Condition(src => src.Tags != null))
                .ForMember(dest => dest.Type, opt => opt.Condition(src => src.Type != null))
                .ForMember(dest => dest.MelangerQuestions, opt => opt.Condition(src => src.MelangerQuestions != false || true)) // bool, toujours présent
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.DureeEstimee, opt => opt.Ignore());



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
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dest => dest.Explication, opt => opt.MapFrom(src => src.Explication ?? string.Empty));

            CreateMap<UpdateQCMOptionDto, QCMOption>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}

