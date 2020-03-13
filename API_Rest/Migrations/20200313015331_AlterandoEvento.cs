using Microsoft.EntityFrameworkCore.Migrations;

namespace API_Rest.Migrations
{
    public partial class AlterandoEvento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ingressos",
                table: "Eventos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ingressos",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
