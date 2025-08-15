namespace SprintQuiz.Api.DTOs
{
    public class QCMOptionDto
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Texte { get; set; } = string.Empty;
        public bool EstCorrecte { get; set; }
    }

    public class CreateQCMOptionDto
    {
        public string Texte { get; set; } = string.Empty;
        public bool EstCorrecte { get; set; }
    }

    public class UpdateQCMOptionDto
    {
        public string? Texte { get; set; }
        public bool? EstCorrecte { get; set; }
    }

    

}

