namespace SprintQuiz.Api.DTOs
{
    // DTOs/DashboardAdminDtos.cs

    public class DashboardOverviewDto
    {
        public int TotalUtilisateurs { get; set; }
        public int ActifsCetteSemaine { get; set; }
        public double TauxEngagement { get; set; }
        public float ProgressionMoyenneGlobale { get; set; }
        public int TempsMoyenRevisionHebdo { get; set; }
        public int NouveauxUtilisateurs { get; set; }
    }

    public class DashboardProgressionDto
    {
        public string Nom { get; set; } = string.Empty;
        public float ProgressionMoyenne { get; set; }
        public int Completions { get; set; }
        public int EnCours { get; set; }
        public int NonCommences { get; set; }
    }

    public class QuizAnalyseDto
    {
        public string QuizTitre { get; set; } = string.Empty;
        public float TauxReussiteGlobal { get; set; }
        public float MoyenneScore { get; set; }
        public int NombreTentatives { get; set; }
        public List<QuestionAnalyseDto> QuestionsLesPlusRatees { get; set; } = new();
        public int[] DistributionScores { get; set; } = new int[5];
    }

    public class QuestionAnalyseDto
    {
        public string Intitule { get; set; } = string.Empty;
        public int TauxErreur { get; set; } // en %
    }

    public class AlerteInactifsDto
    {
        public List<UtilisateurDashboardDto> InactifsDepuisPlusDeNJours { get; set; } = new();
    }

    public class AlerteDifficulteDto
    {
        public List<UtilisateurDashboardDto> UtilisateursFaibleProgression { get; set; } = new();
    }

    public class HistoriqueActiviteDto
    {
        public List<HistoriqueJourDto> ActiviteJournaliere { get; set; } = new();
    }

    public class HistoriqueJourDto
    {
        public DateTime Date { get; set; }
        public int UtilisateursActifs { get; set; }
        public int TentativesQuiz { get; set; }
    }

    public class TopFlopDto
    {
        public TopSectionDto Top { get; set; } = new();
        public FlopSectionDto Flop { get; set; } = new();
    }

    public class TopSectionDto
    {
        public TopFlopItemDto? QuizPlusReussi { get; set; }
        public TopFlopItemDto? ModulePlusConsulte { get; set; }
    }

    public class FlopSectionDto
    {
        public TopFlopItemDto? QuizPlusRate { get; set; }
        public TopFlopItemDto? ModuleMoinsComplet { get; set; }
    }

    public class TopFlopItemDto
    {
        public string Titre { get; set; } = string.Empty;
        public int Valeur { get; set; }
    }

    public class UtilisateurDashboardDto
    {
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public float ProgressionGlobale { get; set; }
        public DateTime? DerniereActivite { get; set; }
    }
}
