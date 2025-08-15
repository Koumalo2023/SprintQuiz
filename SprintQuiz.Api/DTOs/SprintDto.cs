namespace SprintQuiz.Api.DTOs
{
    public class SprintDto
    {
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Ordre { get; set; }
        public List<ModuleDto>? Modules { get; set; }
    }

    public class CreateSprintDto
    {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Ordre { get; set; }
    }

    public class UpdateSprintDto
    {
        public string? Nom { get; set; }
        public string? Description { get; set; }
        public int? Ordre { get; set; }
    }
}

