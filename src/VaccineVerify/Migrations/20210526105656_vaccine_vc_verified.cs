using Microsoft.EntityFrameworkCore.Migrations;

namespace VaccineVerify.Migrations
{
    public partial class vaccine_vc_verified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "VerifiedVaccinationsData",
                newName: "VaccinationDate");

            migrationBuilder.RenameColumn(
                name: "LicenseType",
                table: "VerifiedVaccinationsData",
                newName: "TotalNumberOfDoses");

            migrationBuilder.RenameColumn(
                name: "LicenseIssuedAt",
                table: "VerifiedVaccinationsData",
                newName: "NumberOfDoses");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "VerifiedVaccinationsData",
                newName: "MedicinalProductCode");

            migrationBuilder.AddColumn<string>(
                name: "CountryOfVaccination",
                table: "VerifiedVaccinationsData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FamilyName",
                table: "VerifiedVaccinationsData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "VerifiedVaccinationsData",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryOfVaccination",
                table: "VerifiedVaccinationsData");

            migrationBuilder.DropColumn(
                name: "FamilyName",
                table: "VerifiedVaccinationsData");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "VerifiedVaccinationsData");

            migrationBuilder.RenameColumn(
                name: "VaccinationDate",
                table: "VerifiedVaccinationsData",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "TotalNumberOfDoses",
                table: "VerifiedVaccinationsData",
                newName: "LicenseType");

            migrationBuilder.RenameColumn(
                name: "NumberOfDoses",
                table: "VerifiedVaccinationsData",
                newName: "LicenseIssuedAt");

            migrationBuilder.RenameColumn(
                name: "MedicinalProductCode",
                table: "VerifiedVaccinationsData",
                newName: "FirstName");
        }
    }
}
