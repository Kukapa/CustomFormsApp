using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CustomFormsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFilledFormsToTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilledFormModelId",
                table: "Answers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FilledForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TemplateId = table.Column<int>(type: "integer", nullable: false),
                    DateFilled = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilledForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilledForms_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_FilledFormModelId",
                table: "Answers",
                column: "FilledFormModelId");

            migrationBuilder.CreateIndex(
                name: "IX_FilledForms_TemplateId",
                table: "FilledForms",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_FilledForms_FilledFormModelId",
                table: "Answers",
                column: "FilledFormModelId",
                principalTable: "FilledForms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_FilledForms_FilledFormModelId",
                table: "Answers");

            migrationBuilder.DropTable(
                name: "FilledForms");

            migrationBuilder.DropIndex(
                name: "IX_Answers_FilledFormModelId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "FilledFormModelId",
                table: "Answers");
        }
    }
}
