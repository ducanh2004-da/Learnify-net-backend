using Microsoft.EntityFrameworkCore;
using Learnify.Repository.Models;

namespace Learnify.Repository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DiamondTransaction> DiamondTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                // Map tất cả columns theo Prisma (lowercase)
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.GoogleId).HasColumnName("googleId");
                entity.Property(e => e.PhoneNumber).HasColumnName("phoneNumber");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Role).HasColumnName("role").HasConversion<string>();
                entity.Property(e => e.HashedRefreshToken).HasColumnName("hashedRefreshToken");
                entity.Property(e => e.Avatar).HasColumnName("avatar");
                entity.Property(e => e.IsVerified).HasColumnName("isVerified");
                entity.Property(e => e.CurrentSteak).HasColumnName("currentSteak");
                entity.Property(e => e.LongestSteak).HasColumnName("longestSteak");
                entity.Property(e => e.Diamond).HasColumnName("diamond");
                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
                entity.Property(e => e.DeleteAt).HasColumnName("deletedAt");

                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
            });

            modelBuilder.Entity<DiamondTransaction>(entity =>
            {
                entity.ToTable("DiamondTransaction");

                // Map tất cả columns theo Prisma (lowercase)
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.Amount).HasColumnName("amount");
                entity.Property(e => e.Type).HasColumnName("type");
                entity.Property(e => e.Source).HasColumnName("source");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.Type)
                      .HasColumnName("type")
                      .HasColumnType("\"DiamondTransactionType\"");
                entity.Property(e => e.Source)
                      .HasColumnName("source")
                      .HasColumnType("\"DiamondSource\"");
                entity.HasOne(d => d.User)
                      .WithMany(u => u.DiamondTransactions)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}