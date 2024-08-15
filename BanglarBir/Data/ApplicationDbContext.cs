using BanglarBir.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BanglarBir.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Victim> Victims { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
    }
}
