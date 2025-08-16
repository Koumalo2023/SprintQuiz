using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Formations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    EstActif = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DateOuverture = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Objectifs = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Resume = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NotionsCles = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DureeEstimee = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DifficulteMoyenne = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    NombreQuiz = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NombreFlashcards = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NombreExercices = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DerniereModification = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MotDePasse = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    DateInscription = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    EstActif = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DateOuverture = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Objectifs = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Resume = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NotionsCles = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DureeEstimee = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DifficulteMoyenne = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    NombreQuiz = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NombreFlashcards = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NombreExercices = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DerniereModification = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sprints_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StatistiquesGlobales",
                columns: table => new
                {
                    UtilisateurId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalQuiz = table.Column<int>(type: "integer", nullable: false),
                    MoyenneScore = table.Column<float>(type: "real", nullable: false),
                    TauxReussite = table.Column<float>(type: "real", nullable: false),
                    TempsTotalRevision = table.Column<TimeSpan>(type: "interval", nullable: false),
                    QuestionsQRConsultees = table.Column<int>(type: "integer", nullable: false),
                    CoursTermines = table.Column<int>(type: "integer", nullable: false),
                    ModulesTermines = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatistiquesGlobales", x => x.UtilisateurId);
                    table.ForeignKey(
                        name: "FK_StatistiquesGlobales_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SprintId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    EstActif = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DateOuverture = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Objectifs = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Resume = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NotionsCles = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DureeEstimee = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DifficulteMoyenne = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    NombreQuiz = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NombreFlashcards = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NombreExercices = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DerniereModification = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    EstActif = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DateOuverture = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Objectifs = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Resume = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NotionsCles = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DureeEstimee = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DifficulteMoyenne = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    NombreQuiz = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NombreFlashcards = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NombreExercices = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DerniereModification = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cours_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Enonce = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Solution = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Niveau = table.Column<string>(type: "text", nullable: false),
                    NiveauId = table.Column<Guid>(type: "uuid", nullable: false),
                    NiveauDifficulte = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CoursId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: true),
                    SprintId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercices_Cours_CoursId",
                        column: x => x.CoursId,
                        principalTable: "Cours",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercices_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercices_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProgressionsUtilisateur",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UtilisateurId = table.Column<Guid>(type: "uuid", nullable: false),
                    Niveau = table.Column<string>(type: "text", nullable: false),
                    NiveauId = table.Column<Guid>(type: "uuid", nullable: false),
                    PourcentageComplet = table.Column<float>(type: "real", nullable: false),
                    DerniereActivite = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SprintId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoursId = table.Column<Guid>(type: "uuid", nullable: true),
                    FormationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressionsUtilisateur", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressionsUtilisateur_Cours_CoursId",
                        column: x => x.CoursId,
                        principalTable: "Cours",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProgressionsUtilisateur_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProgressionsUtilisateur_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProgressionsUtilisateur_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProgressionsUtilisateur_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QAQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Question = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Reponse = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Niveau = table.Column<string>(type: "text", nullable: false),
                    NiveauId = table.Column<Guid>(type: "uuid", nullable: false),
                    NiveauDifficulte = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SprintId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoursId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QAQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QAQuestions_Cours_CoursId",
                        column: x => x.CoursId,
                        principalTable: "Cours",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QAQuestions_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QAQuestions_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Niveau = table.Column<string>(type: "text", nullable: false),
                    NiveauId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    SprintId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoursId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Cours_CoursId",
                        column: x => x.CoursId,
                        principalTable: "Cours",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quizzes_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quizzes_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConsultationsExercice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UtilisateurId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateConsultation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MarqueeCompris = table.Column<bool>(type: "boolean", nullable: true),
                    AConsulteSolution = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AUtiliseIndices = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TentativesAvantSolution = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationsExercice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationsExercice_Exercices_ExerciceId",
                        column: x => x.ExerciceId,
                        principalTable: "Exercices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultationsExercice_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EtapesResolution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapesResolution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EtapesResolution_Exercices_ExerciceId",
                        column: x => x.ExerciceId,
                        principalTable: "Exercices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Indices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    Texte = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indices_Exercices_ExerciceId",
                        column: x => x.ExerciceId,
                        principalTable: "Exercices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationsQA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UtilisateurId = table.Column<Guid>(type: "uuid", nullable: false),
                    QAQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateConsultation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MarqueeComprise = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationsQA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationsQA_QAQuestions_QAQuestionId",
                        column: x => x.QAQuestionId,
                        principalTable: "QAQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultationsQA_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QCMQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    Intitule = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    NiveauDifficulte = table.Column<string>(type: "text", nullable: false),
                    Explication = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCMQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QCMQuestions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TentativesQuiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UtilisateurId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    Reussi = table.Column<bool>(type: "boolean", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TempsPasse = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TentativesQuiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TentativesQuiz_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TentativesQuiz_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QCMOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Texte = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EstCorrecte = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCMOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QCMOptions_QCMQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QCMQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReponsesUtilisateurQCM",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TentativeId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstCorrecte = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReponsesUtilisateurQCM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateurQCM_QCMOptions_OptionId",
                        column: x => x.OptionId,
                        principalTable: "QCMOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateurQCM_QCMQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QCMQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateurQCM_TentativesQuiz_TentativeId",
                        column: x => x.TentativeId,
                        principalTable: "TentativesQuiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsExercice_DateConsultation",
                table: "ConsultationsExercice",
                column: "DateConsultation");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsExercice_ExerciceId",
                table: "ConsultationsExercice",
                column: "ExerciceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsExercice_UtilisateurId",
                table: "ConsultationsExercice",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsExercice_UtilisateurId_ExerciceId_DateConsulta~",
                table: "ConsultationsExercice",
                columns: new[] { "UtilisateurId", "ExerciceId", "DateConsultation" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsQA_DateConsultation",
                table: "ConsultationsQA",
                column: "DateConsultation");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsQA_QAQuestionId",
                table: "ConsultationsQA",
                column: "QAQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationsQA_UtilisateurId",
                table: "ConsultationsQA",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Cours_ModuleId_Ordre",
                table: "Cours",
                columns: new[] { "ModuleId", "Ordre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EtapesResolution_ExerciceId",
                table: "EtapesResolution",
                column: "ExerciceId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_CoursId",
                table: "Exercices",
                column: "CoursId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_ModuleId",
                table: "Exercices",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_Niveau_NiveauId",
                table: "Exercices",
                columns: new[] { "Niveau", "NiveauId" });

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_SprintId",
                table: "Exercices",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_Type",
                table: "Exercices",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Formations_Ordre",
                table: "Formations",
                column: "Ordre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Indices_ExerciceId",
                table: "Indices",
                column: "ExerciceId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_SprintId_Ordre",
                table: "Modules",
                columns: new[] { "SprintId", "Ordre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionsUtilisateur_CoursId",
                table: "ProgressionsUtilisateur",
                column: "CoursId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionsUtilisateur_FormationId",
                table: "ProgressionsUtilisateur",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionsUtilisateur_ModuleId",
                table: "ProgressionsUtilisateur",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionsUtilisateur_SprintId",
                table: "ProgressionsUtilisateur",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressionsUtilisateur_UtilisateurId_Niveau_NiveauId",
                table: "ProgressionsUtilisateur",
                columns: new[] { "UtilisateurId", "Niveau", "NiveauId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QAQuestions_CoursId",
                table: "QAQuestions",
                column: "CoursId");

            migrationBuilder.CreateIndex(
                name: "IX_QAQuestions_ModuleId",
                table: "QAQuestions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_QAQuestions_Niveau_NiveauId",
                table: "QAQuestions",
                columns: new[] { "Niveau", "NiveauId" });

            migrationBuilder.CreateIndex(
                name: "IX_QAQuestions_SprintId",
                table: "QAQuestions",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_QCMOptions_QuestionId",
                table: "QCMOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QCMQuestions_QuizId",
                table: "QCMQuestions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CoursId",
                table: "Quizzes",
                column: "CoursId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_ModuleId",
                table: "Quizzes",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Niveau_NiveauId",
                table: "Quizzes",
                columns: new[] { "Niveau", "NiveauId" });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_SprintId",
                table: "Quizzes",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateurQCM_OptionId",
                table: "ReponsesUtilisateurQCM",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateurQCM_QuestionId",
                table: "ReponsesUtilisateurQCM",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateurQCM_TentativeId",
                table: "ReponsesUtilisateurQCM",
                column: "TentativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_FormationId",
                table: "Sprints",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_Ordre",
                table: "Sprints",
                column: "Ordre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_Date",
                table: "TentativesQuiz",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_QuizId",
                table: "TentativesQuiz",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_UtilisateurId",
                table: "TentativesQuiz",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Email",
                table: "Utilisateurs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultationsExercice");

            migrationBuilder.DropTable(
                name: "ConsultationsQA");

            migrationBuilder.DropTable(
                name: "EtapesResolution");

            migrationBuilder.DropTable(
                name: "Indices");

            migrationBuilder.DropTable(
                name: "ProgressionsUtilisateur");

            migrationBuilder.DropTable(
                name: "ReponsesUtilisateurQCM");

            migrationBuilder.DropTable(
                name: "StatistiquesGlobales");

            migrationBuilder.DropTable(
                name: "QAQuestions");

            migrationBuilder.DropTable(
                name: "Exercices");

            migrationBuilder.DropTable(
                name: "QCMOptions");

            migrationBuilder.DropTable(
                name: "TentativesQuiz");

            migrationBuilder.DropTable(
                name: "QCMQuestions");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "Cours");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Sprints");

            migrationBuilder.DropTable(
                name: "Formations");
        }
    }
}
