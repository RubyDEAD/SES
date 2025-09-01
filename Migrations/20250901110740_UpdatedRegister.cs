using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SES.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnrollmentDate",
                table: "Students",
                newName: "DateCreated");

            migrationBuilder.AddColumn<int>(
                name: "MaxEnrollies",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxEnrollies",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Students",
                newName: "EnrollmentDate");
        }
    }
}
