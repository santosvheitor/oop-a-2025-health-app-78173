using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HealthApp.Data.Data;

public class HospitalContextFactory : IDesignTimeDbContextFactory<HospitalContext>
{
    public HospitalContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HospitalContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=FinalHospitalDB;User Id=SA;Password=MyStrongPass123;TrustServerCertificate=True;");

        return new HospitalContext(optionsBuilder.Options);
    }
}