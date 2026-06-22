using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class LinkDriverToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Drivers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_ApplicationUserId",
                table: "Drivers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Users_ApplicationUserId",
                table: "Drivers",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Users_ApplicationUserId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_ApplicationUserId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Drivers");
        }
    }
}
