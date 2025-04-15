using api.Core.Entities;
using api.Core.Entities.SaaS;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<ConnectionConfig> ConnectionConfigs { get; set; }
        
        // SAAS Entities
        public DbSet<Client> Clients { get; set; }
        public DbSet<UserRef> Users { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<ClientPlan> ClientPlans { get; set; }
        public DbSet<ClientRenewal> ClientRenewals { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure ConnectionConfig entity
            modelBuilder.Entity<ConnectionConfig>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.DbType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ServerAddress).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Port).HasMaxLength(10).IsRequired();
                entity.Property(e => e.DatabaseName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });
            
            // Configure SAAS entities
            
            // Client entity
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CountryCode).IsRequired().HasMaxLength(2);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                
                // One-to-many relationships
                entity.HasMany(c => c.Users)
                      .WithOne(u => u.Client)
                      .HasForeignKey(u => u.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(c => c.Plans)
                      .WithOne(cp => cp.Client)
                      .HasForeignKey(cp => cp.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(c => c.Renewals)
                      .WithOne(cr => cr.Client)
                      .HasForeignKey(cr => cr.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // UserRef entity
            modelBuilder.Entity<UserRef>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordSalt).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                
                // Unique constraint on email
                entity.HasIndex(u => u.Email).IsUnique();
            });
            
            // Plan entity
            modelBuilder.Entity<Plan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MonthlyPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.AnnualPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                
                // One-to-many relationship
                entity.HasMany(p => p.ClientPlans)
                      .WithOne(cp => cp.Plan)
                      .HasForeignKey(cp => cp.PlanId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // ClientPlan entity
            modelBuilder.Entity<ClientPlan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.BillingCycle).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.AutoRenew).HasDefaultValue(true);
                
                // One-to-many relationship
                entity.HasMany(cp => cp.Renewals)
                      .WithOne(cr => cr.ClientPlan)
                      .HasForeignKey(cr => cr.ClientPlanId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // ClientRenewal entity
            modelBuilder.Entity<ClientRenewal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.PreviousEndDate).IsRequired();
                entity.Property(e => e.NewStartDate).IsRequired();
                entity.Property(e => e.NewEndDate).IsRequired();
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });
            
            // PaymentMethod entity
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsDefault).HasDefaultValue(false);
            });
        }
    }
}