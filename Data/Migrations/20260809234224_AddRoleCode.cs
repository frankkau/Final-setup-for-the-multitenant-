using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CouncilRevenueCollection.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleCode",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleCode",
                table: "AspNetRoles");
        }
    }
}
