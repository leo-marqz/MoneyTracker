
using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyTracker.Models;

namespace MoneyTracker.Database
{
    public class MoneyTrackerDbContext : IdentityDbContext<User, Role, int>
    {
        public MoneyTrackerDbContext(DbContextOptions options) : base(options)
        {
        }

        // public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
        {
            base.ConfigureConventions(configuration);
            configuration.Properties<decimal>().HavePrecision(18, 2);
            // configuration.Properties<DateTime>().HaveColumnType("date");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}