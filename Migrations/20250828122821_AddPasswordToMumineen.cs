using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITSAssignment.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordToMumineen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Mumineen",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Mumineen");
        }
    }
}
