using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizVersionToTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Supprimer la colonne Version existante (bytea)
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Quizzes");

            // 2. Ajouter une nouvelle colonne Version de type timestamptz avec DEFAULT
            migrationBuilder.AddColumn<DateTime>(
                name: "Version",
                table: "Quizzes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Supprimer la colonne DateTime
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Quizzes");

            // 2. Recréer la colonne bytea avec rowversion
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Quizzes",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]); // ou null si tu veux, mais EF attend une valeur
        }
    }
}
