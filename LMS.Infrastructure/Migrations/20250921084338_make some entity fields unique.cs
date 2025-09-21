using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class makesomeentityfieldsunique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                schema: "lms",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Title",
                schema: "lms",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                schema: "lms",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                schema: "lms",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Title",
                schema: "lms",
                table: "Courses",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "lms",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                schema: "lms",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Title",
                schema: "lms",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                schema: "lms",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                schema: "lms",
                table: "Tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Title",
                schema: "lms",
                table: "Courses",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "lms",
                table: "Categories",
                column: "Name");
        }
    }
}
