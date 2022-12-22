using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quarter.Migrations
{
    public partial class housePosterDescAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosterDesc",
                table: "Houses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterDesc",
                table: "Houses");
        }
    }
}
