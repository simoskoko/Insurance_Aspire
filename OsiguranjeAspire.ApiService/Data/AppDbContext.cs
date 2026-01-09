using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using OsiguranjeAspire.ApiService.Models;

namespace OsiguranjeAspire.ApiService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{
    public DbSet<User> Users => Set<User>();
}
