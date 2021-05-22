using Microsoft.EntityFrameworkCore.Migrations;

namespace VaccineVerify.Migrations
{
    public partial class vaccine_vc_verify_init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VaccinationDataPresentationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DidId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MattrPresentationTemplateReponse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationDataPresentationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaccinationDataPresentationVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DidId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvokePresentationResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Did = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignAndEncodePresentationRequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Challenge = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationDataPresentationVerifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerifiedVaccinationsData",
                columns: table => new
                {
                    ChallengeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PresentationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimsId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseIssuedAt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Verified = table.Column<bool>(type: "bit", nullable: false),
                    Holder = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifiedVaccinationsData", x => x.ChallengeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VaccinationDataPresentationTemplates");

            migrationBuilder.DropTable(
                name: "VaccinationDataPresentationVerifications");

            migrationBuilder.DropTable(
                name: "VerifiedVaccinationsData");
        }
    }
}
