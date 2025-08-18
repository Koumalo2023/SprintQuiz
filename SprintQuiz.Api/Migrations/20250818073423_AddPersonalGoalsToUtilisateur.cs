using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalGoalsToUtilisateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ObjectifHebdomadaireExercices",
                table: "Utilisateurs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ObjectifHebdomadaireFlashcards",
                table: "Utilisateurs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ObjectifHebdomadaireQuiz",
                table: "Utilisateurs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ObjectifTempsRevision",
                table: "Utilisateurs",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectifHebdomadaireExercices",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "ObjectifHebdomadaireFlashcards",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "ObjectifHebdomadaireQuiz",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "ObjectifTempsRevision",
                table: "Utilisateurs");
        }
    }
}
