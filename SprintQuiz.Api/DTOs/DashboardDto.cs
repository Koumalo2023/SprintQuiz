namespace SprintQuiz.Api.DTOs
{
    // DTOs/DashboardDto.cs
    public class DashboardDto
    {
        // --- Progression globale ---
        public double ProgressionGlobale { get; set; } // 0.0 à 1.0
        public string DerniereSession { get; set; } = string.Empty; // "Aujourd'hui à 14h30"
        public int TempsRevisionAujourdhui { get; set; } // en minutes
        public int Streak { get; set; } // jours consécutifs

        // --- Objectifs ---
        public DashboardObjectifDto ObjectifHebdomadaireQuiz { get; set; } = new();
        public DashboardObjectifDto ObjectifHebdomadaireFlashcards { get; set; } = new();
        public DashboardObjectifDto ObjectifHebdomadaireExercices { get; set; } = new();
        public DashboardObjectifTempsDto ObjectifTempsRevision { get; set; } = new();

        // --- Prochaines révisions ---
        public List<DashboardRevisionItemDto> ProchainesRevisions { get; set; } = new();

        // --- Historique (derniers 7 jours) ---
        public List<DashboardJourDto> HistoriqueRevision { get; set; } = new();
    }

    public class DashboardObjectifDto
    {
        public int Objectif { get; set; }
        public int Realise { get; set; }
        public double Pourcentage => Objectif > 0 ? (double)Realise / Objectif : 0;
    }

    public class DashboardObjectifTempsDto
    {
        public TimeSpan Objectif { get; set; } // ex: 5h
        public TimeSpan Realise { get; set; } // ex: 3h
        public double Pourcentage => Objectif.TotalMinutes > 0 ? Realise.TotalMinutes / Objectif.TotalMinutes : 0;
    }

    public class DashboardRevisionItemDto
    {
        public string Type { get; set; } = string.Empty; // "quiz", "flashcard", "exercice"
        public string Nom { get; set; } = string.Empty;
        public string Niveau { get; set; } = string.Empty; // "Sprint 2", "Module 3"
        public string Priorite { get; set; } = string.Empty; // "haute", "moyenne", "basse"
    }

    public class DashboardJourDto
    {
        public DateTime Date { get; set; }
        public int Minutes { get; set; }
    }


    
}
