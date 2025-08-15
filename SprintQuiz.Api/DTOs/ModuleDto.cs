namespace SprintQuiz.Api.DTOs
{
    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Ordre { get; set; }
        public Guid SprintId { get; set; }
        public string? SprintNom { get; set; }
        public List<CoursDto>? Cours { get; set; }
    }

    public class CreateModuleDto
    {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Ordre { get; set; }
        public Guid SprintId { get; set; }
    }

    public class UpdateModuleDto
    {
        public string? Nom { get; set; }
        public string? Description { get; set; }
        public int? Ordre { get; set; }
        public Guid? SprintId { get; set; }
    }
}

