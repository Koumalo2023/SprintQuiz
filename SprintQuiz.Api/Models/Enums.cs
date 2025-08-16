namespace SprintQuiz.Api.Models
{
    public enum NiveauEnum
    {
        Cours,
        Module,
        Sprint,
        Formation
    }

    public enum NiveauDifficulte
    {
        Facile,
        Moyen,
        Difficile
    }

    public enum RoleUtilisateur
    {
        Admin,
        Etudiant
    }

    public enum TypeExercice
    {
        Basique,       // Exercice simple de compréhension
        Applique,      // Application directe d’un concept
        Analyse,       // Analyse de code, situation, problème
        Cas,           // Étude de cas complète (projet mini)
        Defi           // Défi bonus (optionnel, difficile)
    }
}

