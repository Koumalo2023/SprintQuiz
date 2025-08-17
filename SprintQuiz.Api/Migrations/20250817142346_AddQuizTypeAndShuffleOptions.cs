using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizTypeAndShuffleOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DerniereModification",
                table: "Quizzes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DureeEstimee",
                table: "Quizzes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MelangerQuestions",
                table: "Quizzes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "Tags",
                table: "Quizzes",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Quizzes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DerniereModification",
                table: "QAQuestions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DureeEstimee",
                table: "QAQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "ProgressionsUtilisateur",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DerniereModification",
                table: "Exercices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DureeEstimee",
                table: "Exercices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DerniereModification",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "DureeEstimee",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "MelangerQuestions",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "DerniereModification",
                table: "QAQuestions");

            migrationBuilder.DropColumn(
                name: "DureeEstimee",
                table: "QAQuestions");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "ProgressionsUtilisateur");

            migrationBuilder.DropColumn(
                name: "DerniereModification",
                table: "Exercices");

            migrationBuilder.DropColumn(
                name: "DureeEstimee",
                table: "Exercices");
        }
    }
}
