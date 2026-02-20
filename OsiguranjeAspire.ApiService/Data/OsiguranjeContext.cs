using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using OsiguranjeAspire.ApiService.Models;

namespace OsiguranjeAspire.ApiService.Data
{
    public class OsiguranjeContext : DbContext
    {
        public OsiguranjeContext(DbContextOptions<OsiguranjeContext> opts) : base(opts){}

        public DbSet<Polisa> Polise => Set<Polisa>();
    }
}
