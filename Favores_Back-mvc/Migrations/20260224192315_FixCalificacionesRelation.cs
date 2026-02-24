using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Favores_Back_mvc.Migrations
{
    /// <inheritdoc />
    public partial class FixCalificacionesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_FavorId",
                table: "Calificaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_FavorId_EvaluadorId",
                table: "Calificaciones",
                columns: new[] { "FavorId", "EvaluadorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_FavorId_EvaluadorId",
                table: "Calificaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_FavorId",
                table: "Calificaciones",
                column: "FavorId",
                unique: true);
        }
    }
}
