using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.DTOs
{



    public class CreateCoursDto
    {
        [Required] public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required] public int Ordre { get; set; }
        [Required] public Guid ModuleId { get; set; }

        public bool EstActif { get; set; } = true;
        public DateTime? DateOuverture { get; set; }
        public string? Objectifs { get; set; }
        public string? Resume { get; set; }
        public string? NotionsCles { get; set; }
        public List<string> Tags { get; set; } = new();
    }


    public class UpdateCoursDto
    {
        public string? Nom { get; set; }
        public string? Description { get; set; }
        public int? Ordre { get; set; }
        public Guid? ModuleId { get; set; }

        public bool? EstActif { get; set; }
        public DateTime? DateOuverture { get; set; }
        public string? Objectifs { get; set; }
        public string? Resume { get; set; }
        public string? NotionsCles { get; set; }
        public List<string>? Tags { get; set; }
    }
}

