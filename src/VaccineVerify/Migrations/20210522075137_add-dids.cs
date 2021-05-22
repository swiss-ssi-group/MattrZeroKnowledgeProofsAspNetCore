using Microsoft.EntityFrameworkCore.Migrations;

namespace VaccineVerify.Migrations
{
    public partial class adddids : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dids",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DidId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DidTypeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DidData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dids", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dids");
        }
    }
}
