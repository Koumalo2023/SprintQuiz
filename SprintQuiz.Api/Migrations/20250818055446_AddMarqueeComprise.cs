using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SprintQuiz.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMarqueeComprise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstCompris",
                table: "QAQuestions",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstCompris",
                table: "QAQuestions");
        }
    }
}
