using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExerciceNavigationForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercices_Cours_NiveauId",
                table: "Exercices");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercices_Modules_NiveauId",
                table: "Exercices");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercices_Sprints_NiveauId",
                table: "Exercices");

            migrationBuilder.DropIndex(
                name: "IX_Exercices_NiveauId",
                table: "Exercices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Exercices_NiveauId",
                table: "Exercices",
                column: "NiveauId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercices_Cours_NiveauId",
                table: "Exercices",
                column: "NiveauId",
                principalTable: "Cours",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercices_Modules_NiveauId",
                table: "Exercices",
                column: "NiveauId",
                principalTable: "Modules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercices_Sprints_NiveauId",
                table: "Exercices",
                column: "NiveauId",
                principalTable: "Sprints",
                principalColumn: "Id");
        }
    }
}
