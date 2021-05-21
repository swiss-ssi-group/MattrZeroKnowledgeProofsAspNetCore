using Microsoft.EntityFrameworkCore;

namespace VaccineCredentialsIssuer.Data
{
    public class VaccinationDataMattrContext : DbContext
    {
        public VaccinationDataMattrContext(DbContextOptions<VaccinationDataMattrContext> options) : base(options)
        { }

        public DbSet<VaccinationDataCredentials> VaccinationDataCredentials { get; set; }
    }
}
