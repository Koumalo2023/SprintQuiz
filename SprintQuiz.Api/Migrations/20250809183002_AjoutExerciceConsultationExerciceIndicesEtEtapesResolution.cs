using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class AjoutExerciceConsultationExerciceIndicesEtEtapesResolution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercices_Cours_NiveauId",
                        column: x => x.NiveauId,
                        principalTable: "Cours",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercices_Modules_NiveauId",
                        column: x => x.NiveauId,
                        principalTable: "Modules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercices_Sprints_NiveauId",
                        column: x => x.NiveauId,
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
                name: "IX_EtapesResolution_ExerciceId",
                table: "EtapesResolution",
                column: "ExerciceId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_Niveau_NiveauId",
                table: "Exercices",
                columns: new[] { "Niveau", "NiveauId" });

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_NiveauId",
                table: "Exercices",
                column: "NiveauId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_Type",
                table: "Exercices",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Indices_ExerciceId",
                table: "Indices",
                column: "ExerciceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultationsExercice");

            migrationBuilder.DropTable(
                name: "EtapesResolution");

            migrationBuilder.DropTable(
                name: "Indices");

            migrationBuilder.DropTable(
                name: "Exercices");
        }
    }
}
