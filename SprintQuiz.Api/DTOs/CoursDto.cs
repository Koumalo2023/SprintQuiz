namespace SprintQuiz.Api.DTOs
{
    public class CoursDto
    {
        public Guid Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Ordre { get; set; }
        public Guid ModuleId { get; set; }
        public string? ModuleNom { get; set; }
    }

    public class CreateCoursDto
    {
        public string Titre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Ordre { get; set; }
        public Guid ModuleId { get; set; }
    }

    public class UpdateCoursDto
    {
        public string? Titre { get; set; }
        public string? Description { get; set; }
        public int? Ordre { get; set; }
        public Guid? ModuleId { get; set; }
    }
}

