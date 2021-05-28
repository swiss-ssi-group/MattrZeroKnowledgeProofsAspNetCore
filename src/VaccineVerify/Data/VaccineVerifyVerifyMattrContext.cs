using Microsoft.EntityFrameworkCore;

namespace VaccineVerify.Data
{
    public class VaccineVerifyVerifyMattrContext : DbContext
    {
        public VaccineVerifyVerifyMattrContext(DbContextOptions<VaccineVerifyVerifyMattrContext> options) : base(options)
        { }

        public DbSet<VaccinationDataPresentationTemplate> VaccinationDataPresentationTemplates { get; set; }

        public DbSet<VaccinationDataPresentationVerify> VaccinationDataPresentationVerifications { get; set; }

        public DbSet<VerifiedVaccinationsData> VerifiedVaccinationsData { get; set; }

        public DbSet<Did> Dids { get; set; }


    }
}
