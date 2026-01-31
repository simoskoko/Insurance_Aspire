using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using OsiguranjeAspire.ApiService.Models;

namespace OsiguranjeAspire.ApiService.Data
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext(DbContextOptions<InsuranceDbContext> opts) : base(opts){}

        public DbSet<Polisa> Polise => Set<Polisa>();
    }
}
