using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Favores_Back_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddPostulacionEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Postulaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Postulaciones");
        }
    }
}
