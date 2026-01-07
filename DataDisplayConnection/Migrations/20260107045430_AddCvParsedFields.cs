using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataDisplayConnection.Migrations
{
    /// <inheritdoc />
    public partial class AddCvParsedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ExperienceYears",
                table: "Profiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ExperienceYears",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Profiles");
        }
    }
}
