using SprintQuiz.Api.Models;

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
        public byte[] Version { get; set; }
        public List<QCMQuestionDto>? Questions { get; set; }
    }

    public class CreateQuizDto
    {
        public string Titre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public NiveauEnum Niveau { get; set; }
        public Guid NiveauId { get; set; }
        public List<CreateQCMQuestionDto> Questions { get; set; } = new();
    }

    public class UpdateQuizDto
    {
        public string? Titre { get; set; }
        public string? Description { get; set; }
        public NiveauEnum? Niveau { get; set; }
        public Guid? NiveauId { get; set; } 
        public byte[] Version { get; set; } = Array.Empty<byte>();
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

    

}

