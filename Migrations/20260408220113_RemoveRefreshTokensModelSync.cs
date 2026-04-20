using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRefreshTokensModelSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorProfile_Instructors_InstructorId",
                table: "InstructorProfile");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorProfile",
                table: "InstructorProfile");

            migrationBuilder.RenameTable(
                name: "InstructorProfile",
                newName: "InstructorProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorProfile_InstructorId",
                table: "InstructorProfiles",
                newName: "IX_InstructorProfiles_InstructorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorProfiles",
                table: "InstructorProfiles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorProfiles_Instructors_InstructorId",
                table: "InstructorProfiles",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorProfiles_Instructors_InstructorId",
                table: "InstructorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Students_Email",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorProfiles",
                table: "InstructorProfiles");

            migrationBuilder.RenameTable(
                name: "InstructorProfiles",
                newName: "InstructorProfile");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorProfiles_InstructorId",
                table: "InstructorProfile",
                newName: "IX_InstructorProfile_InstructorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorProfile",
                table: "InstructorProfile",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReplacedByTokenHash = table.Column<string>(type: "TEXT", nullable: true),
                    RevokedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TokenHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorProfile_Instructors_InstructorId",
                table: "InstructorProfile",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
