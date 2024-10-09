using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomFormsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerUserIdToTemplateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Templates_TemplateModelId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_TemplateModelId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TemplateModelId",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "OwnerUserId",
                table: "Templates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TemplateId",
                table: "Questions",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Templates_TemplateId",
                table: "Questions",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Templates_TemplateId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_TemplateId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Templates");

            migrationBuilder.AddColumn<int>(
                name: "TemplateModelId",
                table: "Questions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TemplateModelId",
                table: "Questions",
                column: "TemplateModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Templates_TemplateModelId",
                table: "Questions",
                column: "TemplateModelId",
                principalTable: "Templates",
                principalColumn: "Id");
        }
    }
}
