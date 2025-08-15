using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciceRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CoursId",
                table: "Exercices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "Exercices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SprintId",
                table: "Exercices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_CoursId",
                table: "Exercices",
                column: "CoursId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_ModuleId",
                table: "Exercices",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_SprintId",
                table: "Exercices",
                column: "SprintId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercices_Cours_CoursId",
                table: "Exercices",
                column: "CoursId",
                principalTable: "Cours",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercices_Modules_ModuleId",
                table: "Exercices",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercices_Sprints_SprintId",
                table: "Exercices",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercices_Cours_CoursId",
                table: "Exercices");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercices_Modules_ModuleId",
                table: "Exercices");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercices_Sprints_SprintId",
                table: "Exercices");

            migrationBuilder.DropIndex(
                name: "IX_Exercices_CoursId",
                table: "Exercices");

            migrationBuilder.DropIndex(
                name: "IX_Exercices_ModuleId",
                table: "Exercices");

            migrationBuilder.DropIndex(
                name: "IX_Exercices_SprintId",
                table: "Exercices");

            migrationBuilder.DropColumn(
                name: "CoursId",
                table: "Exercices");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Exercices");

            migrationBuilder.DropColumn(
                name: "SprintId",
                table: "Exercices");
        }
    }
}
