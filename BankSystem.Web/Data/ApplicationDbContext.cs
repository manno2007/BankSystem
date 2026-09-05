using BankSystem.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Account>().Property(a => a.Balance).HasPrecision(18, 2);
            builder.Entity<Transaction>().Property(t => t.Amount).HasPrecision(18, 2);
            builder.Entity<Loan>().Property(l => l.PrincipalAmount).HasPrecision(18, 2);
            builder.Entity<Loan>().Property(l => l.EMI).HasPrecision(18, 2);
            builder.Entity<Loan>().Property(l => l.InterestRate).HasPrecision(5, 2);

            builder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
