﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VaccineVerify.Data;

namespace VaccineVerify.Migrations
{
    [DbContext(typeof(VaccineVerifyVerifyMattrContext))]
    partial class VaccineVerifyVerifyMattrContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VaccineVerify.Data.Did", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DidData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DidId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DidTypeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Dids");
                });

            modelBuilder.Entity("VaccineVerify.Data.VaccinationDataPresentationTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DidId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MattrPresentationTemplateReponse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("VaccinationDataPresentationTemplates");
                });

            modelBuilder.Entity("VaccineVerify.Data.VaccinationDataPresentationVerify", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CallbackUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Challenge")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Did")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DidId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InvokePresentationResponse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SignAndEncodePresentationRequestBody")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("VaccinationDataPresentationVerifications");
                });

            modelBuilder.Entity("VaccineVerify.Data.VerifiedVaccinationsData", b =>
                {
                    b.Property<string>("ChallengeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ClaimsId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryOfVaccination")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DateOfBirth")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FamilyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GivenName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Holder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicinalProductCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NumberOfDoses")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PresentationType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TotalNumberOfDoses")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VaccinationDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Verified")
                        .HasColumnType("bit");

                    b.HasKey("ChallengeId");

                    b.ToTable("VerifiedVaccinationsData");
                });
#pragma warning restore 612, 618
        }
    }
}
