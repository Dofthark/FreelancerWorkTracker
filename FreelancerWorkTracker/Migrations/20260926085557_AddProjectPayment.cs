using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelancerWorkTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Projects");
        }
    }
}
