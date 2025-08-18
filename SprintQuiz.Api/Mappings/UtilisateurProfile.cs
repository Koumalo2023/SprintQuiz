using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class UtilisateurProfile : Profile
    {
        public UtilisateurProfile()
        {
            CreateMap<Utilisateur, UtilisateurDto>();

            CreateMap<CreateUtilisateurDto, Utilisateur>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateInscription, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateUtilisateurDto, Utilisateur>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<InscriptionFormation, InscriptionDto>()
                .ForMember(dest => dest.NomUtilisateur, opt => opt.MapFrom(src => src.Utilisateur.Nom))
                .ForMember(dest => dest.NomFormation, opt => opt.MapFrom(src => src.Formation.Nom));

            CreateMap<CreateInscriptionDto, InscriptionFormation>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateInscription, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<QAQuestion, QAQuestionDto>();

            CreateMap<CreateQAQuestionDto, QAQuestion>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateQAQuestionDto, QAQuestion>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ProgressionUtilisateur, ProgressionUtilisateurDto>()
                .ForMember(dest => dest.NiveauNom, opt => opt.MapFrom(src => 
                    src.Niveau == NiveauEnum.Sprint ? src.Sprint!.Nom :
                    src.Niveau == NiveauEnum.Module ? src.Module!.Nom :
                    src.Cours!.Nom));

            CreateMap<StatistiquesGlobales, StatistiquesGlobalesDto>();

            CreateMap<ConsultationQA, ConsultationQADto>();

            CreateMap<CreateConsultationQADto, ConsultationQA>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateConsultation, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}

