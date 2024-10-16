using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomFormsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionViewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionType",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "IsVisibleInResults",
                table: "Questions",
                newName: "ShowInTable");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "ShowInTable",
                table: "Questions",
                newName: "IsVisibleInResults");

            migrationBuilder.AddColumn<string>(
                name: "QuestionType",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
