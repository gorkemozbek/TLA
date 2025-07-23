using Microsoft.EntityFrameworkCore;
using TLA.Core.Entities;

namespace TLA.Infrastructure.Data
{
    public class TLADbContext : DbContext
    {
        public TLADbContext(DbContextOptions<TLADbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired();
            });

            // Role Configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.RoleName).IsUnique();
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
            });

            // UserRole Configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Permission Configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // RolePermission Configuration
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "user.read", Description = "Read users", CreatedAt = DateTime.UtcNow },
                new Permission { Id = 2, Name = "user.write", Description = "Create/Update users", CreatedAt = DateTime.UtcNow },
                new Permission { Id = 3, Name = "user.delete", Description = "Delete users", CreatedAt = DateTime.UtcNow },
                new Permission { Id = 4, Name = "admin.panel", Description = "Access admin panel", CreatedAt = DateTime.UtcNow }
            );

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin", CreatedAt = DateTime.UtcNow },
                new Role { Id = 2, RoleName = "User", CreatedAt = DateTime.UtcNow }
            );

            // Seed Role Permissions
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1, CreatedAt = DateTime.UtcNow }, // Admin - user.read
                new RolePermission { Id = 2, RoleId = 1, PermissionId = 2, CreatedAt = DateTime.UtcNow }, // Admin - user.write
                new RolePermission { Id = 3, RoleId = 1, PermissionId = 3, CreatedAt = DateTime.UtcNow }, // Admin - user.delete
                new RolePermission { Id = 4, RoleId = 1, PermissionId = 4, CreatedAt = DateTime.UtcNow }, // Admin - admin.panel
                new RolePermission { Id = 5, RoleId = 2, PermissionId = 1, CreatedAt = DateTime.UtcNow }  // User - user.read
            );

            // Seed Default Admin User (password: admin123)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@tla.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Assign Admin Role to Default User
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, UserId = 1, RoleId = 1, CreatedAt = DateTime.UtcNow }
            );
        }
    }
}