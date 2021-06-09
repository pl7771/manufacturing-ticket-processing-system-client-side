using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Ticketing_System_TILE03.Data.Mappers;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Bijlage> Bijlages { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Company> Companies { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //hier gaan we dan verder onze ticket configuratie
            builder.ApplyConfiguration(new TicketConfiguration());
            builder.ApplyConfiguration(new ContractTypeConfiguration());
            builder.ApplyConfiguration(new ContractConfiguration());
            RenameIdentityTables(builder);
        }

        private void RenameIdentityTables(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Identity");
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }

/*        public override int SaveChanges()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries<Ticket>())
            {
                dbEntityEntry.Entity.UpdateDates();
            }
            return base.SaveChanges();
        }*/
    }
}