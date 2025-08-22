using Microsoft.EntityFrameworkCore;
using ProductMaintenance.Entity;

namespace ProductMaintenance.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<UserType> UserTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price).HasPrecision(18, 2);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.LastName)
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.MobileNo)
                      .HasMaxLength(20);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                // Relationship: User has one UserType, UserType has many Users
                entity.HasOne(u => u.UserType)
                      .WithMany(ut => ut.Users)
                      .HasForeignKey(u => u.UserTypeId)
                      .IsRequired();

                entity.Property(u => u.CreatedDate)
                      .IsRequired();

                entity.Property(u => u.UpdatedDate);
            });

            // UserType configuration
            modelBuilder.Entity<UserType>(entity =>
            {
                entity.Property(ut => ut.Name)
                      .IsRequired()
                      .HasMaxLength(50);

                // Seed data
                entity.HasData(
                    new UserType { Id = 1, Name = "Admin" },
                    new UserType { Id = 2, Name = "Manager" }
                );
            });

        }
    }
}
