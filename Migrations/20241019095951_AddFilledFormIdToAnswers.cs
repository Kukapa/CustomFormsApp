using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomFormsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFilledFormIdToAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_FilledForms_FilledFormModelId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_FilledFormModelId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "FilledFormModelId",
                table: "Answers");

            migrationBuilder.AddColumn<int>(
                name: "FilledFormId",
                table: "Answers",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_FilledFormId",
                table: "Answers",
                column: "FilledFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_FilledForms_FilledFormId",
                table: "Answers",
                column: "FilledFormId",
                principalTable: "FilledForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_FilledForms_FilledFormId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_FilledFormId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "FilledFormId",
                table: "Answers");

            migrationBuilder.AddColumn<int>(
                name: "FilledFormModelId",
                table: "Answers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_FilledFormModelId",
                table: "Answers",
                column: "FilledFormModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_FilledForms_FilledFormModelId",
                table: "Answers",
                column: "FilledFormModelId",
                principalTable: "FilledForms",
                principalColumn: "Id");
        }
    }
}
