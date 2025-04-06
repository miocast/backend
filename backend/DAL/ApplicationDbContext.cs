using backend.Contracts;
using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace backend.DAL
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TechnicalSpec> TechnicalSpecs => Set<TechnicalSpec>();
        public DbSet<NpaDocument> NpaDocuments => Set<NpaDocument>();
        public DbSet<DocumentAnalys> DocumentAnalys => Set<DocumentAnalys>();
        public DbSet<DocumentNpa> DocumentNpas => Set<DocumentNpa>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().Property(u => u.Initials).HasMaxLength(5);

            builder.Entity<TechnicalSpec>().Property(ts => ts.Name).IsRequired().HasMaxLength(100);
            builder.Entity<TechnicalSpec>().Property(ts => ts.Link).IsRequired().HasMaxLength(100);
            builder.Entity<TechnicalSpec>().Property(ts => ts.UserId).IsRequired().HasMaxLength(100);

            builder.Entity<NpaDocument>().Property(doc => doc.Name).IsRequired().HasMaxLength(100);
            builder.Entity<NpaDocument>().Property(doc => doc.FilePath).IsRequired().HasMaxLength(300);

            builder.HasDefaultSchema("hackathon");
        }

    }
}
