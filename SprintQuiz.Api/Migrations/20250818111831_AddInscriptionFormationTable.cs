using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInscriptionFormationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TentativeQuizId",
                table: "ReponsesUtilisateurQCM",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InscriptionFormations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UtilisateurId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    DateInscription = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscriptionFormations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InscriptionFormations_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InscriptionFormations_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateurQCM_TentativeQuizId",
                table: "ReponsesUtilisateurQCM",
                column: "TentativeQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_InscriptionFormations_FormationId",
                table: "InscriptionFormations",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_InscriptionFormations_UtilisateurId_FormationId",
                table: "InscriptionFormations",
                columns: new[] { "UtilisateurId", "FormationId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReponsesUtilisateurQCM_TentativesQuiz_TentativeQuizId",
                table: "ReponsesUtilisateurQCM",
                column: "TentativeQuizId",
                principalTable: "TentativesQuiz",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReponsesUtilisateurQCM_TentativesQuiz_TentativeQuizId",
                table: "ReponsesUtilisateurQCM");

            migrationBuilder.DropTable(
                name: "InscriptionFormations");

            migrationBuilder.DropIndex(
                name: "IX_ReponsesUtilisateurQCM_TentativeQuizId",
                table: "ReponsesUtilisateurQCM");

            migrationBuilder.DropColumn(
                name: "TentativeQuizId",
                table: "ReponsesUtilisateurQCM");
        }
    }
}
