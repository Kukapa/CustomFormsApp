using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomFormsApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTopicToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Topic",
                table: "Templates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FilledForms_UserId",
                table: "FilledForms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FilledForms_AspNetUsers_UserId",
                table: "FilledForms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilledForms_AspNetUsers_UserId",
                table: "FilledForms");

            migrationBuilder.DropIndex(
                name: "IX_FilledForms_UserId",
                table: "FilledForms");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Templates");
        }
    }
}
