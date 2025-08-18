using SprintQuiz.Api.Models;
using System;

namespace SprintQuiz.Api.DTOs
{
    public class QuizDto
    {
        public Guid Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public NiveauEnum Niveau { get; set; }
        public Guid NiveauId { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime Version { get; set; }
        public int DureeEstimee { get; set; }
        public DateTime? DerniereModification { get; set; }
        public DateTime? DerniereActivite { get; set; }
        public float? MeilleurScore { get; set; }
        public bool MelangerQuestions { get; set; }
        public float? DernierScore { get; set; }
        public int NombreTentatives { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public List<string>? Tags { get; set; }
        public List<QCMQuestionDto>? Questions { get; set; }
    }

    public class CreateQuizDto
    {
        public string Titre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public NiveauEnum Niveau { get; set; }
        public Guid NiveauId { get; set; }
        public TypeQuiz Type { get; set; } = TypeQuiz.Entrainement;
        public bool MelangerQuestions { get; set; } = false;
        public List<string>? Tags { get; set; }
        public List<CreateQCMQuestionDto> Questions { get; set; } = new();
    }

    public class UpdateQuizDto
    {
        public string? Titre { get; set; }
        public string? Description { get; set; }
        public NiveauEnum? Niveau { get; set; }
        public Guid? NiveauId { get; set; }
        public List<string>? Tags { get; set; } 
        public TypeQuiz Type { get; set; }
        public bool MelangerQuestions { get; set; } 
        // Ajout : Mise à jour complète des questions
        public List<CreateQCMQuestionDto>? Questions { get; set; }
    }

    public class QuizResultDto
    {
        public Guid QuizId { get; set; }
        public string QuizTitre { get; set; } = string.Empty;
        public float Score { get; set; }
        public bool Reussi { get; set; }
        public TimeSpan TempsPasse { get; set; }
        public DateTime Date { get; set; }
        public List<ReponseQuestionDto> Reponses { get; set; } = new(); 
        public ResumeSessionDto? ResumeSession { get; set; }
    }

    public class ReponseQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionIntitule { get; set; } = string.Empty;
        public Guid OptionChoisieId { get; set; }
        public string OptionChoisieTexte { get; set; } = string.Empty;
        public bool EstCorrecte { get; set; }
        public string? Explication { get; set; } 
    }

    public class ResumeSessionDto
    {
        public int QuestionsRevues { get; set; }
        public double TauxComprehension { get; set; } // 0.0 à 1.0
        public List<string> PointsForts { get; set; } = new();
        public List<string> PointsFaibles { get; set; } = new();
        public string Conseil { get; set; } = string.Empty;
    }



}

