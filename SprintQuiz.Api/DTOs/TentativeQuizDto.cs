namespace SprintQuiz.Api.DTOs
{
    public class TentativeQuizDto
    {
        public Guid Id { get; set; }
        public Guid UtilisateurId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitre { get; set; } = string.Empty;
        public float Score { get; set; }
        public bool Reussi { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TempsPasse { get; set; }
        // --- Ajouté pour le mode révision ---
        public List<ReponseQuestionDto> Reponses { get; set; } = new();
    }

    public class CreateTentativeQuizDto
    {
        public Guid QuizId { get; set; }
        public List<ReponseUtilisateurDto> Reponses { get; set; } = new();
        public TimeSpan TempsPasse { get; set; }
    }

    public class ReponseUtilisateurDto
    {
        public Guid QuestionId { get; set; }
        public Guid OptionId { get; set; }
    }
}

