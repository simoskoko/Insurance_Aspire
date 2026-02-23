using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using OsiguranjeAspire.ApiService.Models;
using Microsoft.Identity.Client;

namespace OsiguranjeAspire.ApiService.Data
{
    public class OsiguranjeContext : DbContext
    {
        public OsiguranjeContext(DbContextOptions<OsiguranjeContext> opts) : base(opts){}

        public DbSet<Polisa> Polise => Set<Polisa>();

        public DbSet<Zaposleni> Zaposleni => Set<Zaposleni>();
    }
}
