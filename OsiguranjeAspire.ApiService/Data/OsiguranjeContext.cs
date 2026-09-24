using Microsoft.EntityFrameworkCore;
using OsiguranjeAspire.ApiService.Models;

namespace OsiguranjeAspire.ApiService.Data
{
    public class OsiguranjeContext : DbContext
    {
        public OsiguranjeContext(DbContextOptions<OsiguranjeContext> opts) : base(opts){}

        public DbSet<Polisa> Polise => Set<Polisa>();

        public DbSet<Zaposleni> Zaposleni => Set<Zaposleni>();

        public DbSet<SifarnikLob> SifarnikLOB => Set<SifarnikLob>();

        public DbSet<SifarnikVrstaPlacanja> SifarnikVrstaPlacanja => Set<SifarnikVrstaPlacanja>();
    }
}
