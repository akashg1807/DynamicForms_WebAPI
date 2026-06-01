using Microsoft.EntityFrameworkCore;

namespace DynamicForms_WebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Maps our C# classes to physical SQL Server Tables
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormField> FormFields { get; set; }
        public DbSet<FormRolePermission> FormRolePermissions { get; set; }
        public DbSet<FieldRolePermission> FieldRolePermissions { get; set; }
        public DbSet<FormSubmission> FormSubmissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // This tells .NET 9 to ignore the strict model check warning and let the update run
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // 1. SEED ROLES
            // ==========================================
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Manager" },
                new Role { RoleId = 3, RoleName = "User" }
            );

            // ==========================================
            // 2. SEED USERS
            // (Keeping passwords simple for testing; will check against these during login)
            // ==========================================
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Username = "admin_bob", PasswordHash = "password123", RoleId = 1 },
                new User { UserId = 2, Username = "manager_alice", PasswordHash = "password123", RoleId = 2 },
                new User { UserId = 3, Username = "user_charlie", PasswordHash = "password123", RoleId = 3 }
            );

            // ==========================================
            // 3. SEED DYNAMIC FORM METADATA
            // ==========================================
            modelBuilder.Entity<Form>().HasData(
                new Form { FormId = 1, FormName = "Employee Onboarding Form", CreatedBy = 1 }
            );

            // ==========================================
            // 4. SEED FORM FIELDS (Configured via JSON string for rules)
            // ==========================================
            modelBuilder.Entity<FormField>().HasData(
                new FormField { FieldId = 1, FormId = 1, FieldName = "Full Name", FieldType = "Text", IsRequired = true, ValidationRules = "{\"minLength\": 2, \"maxLength\": 50}" },
                new FormField { FieldId = 2, FormId = 1, FieldName = "Joining Date", FieldType = "Date", IsRequired = true, ValidationRules = null },
                new FormField { FieldId = 3, FormId = 1, FieldName = "Remote Worker", FieldType = "Checkbox", IsRequired = false, ValidationRules = null },
                new FormField { FieldId = 4, FormId = 1, FieldName = "Approved Salary", FieldType = "Number", IsRequired = true, ValidationRules = "{\"min\": 1000}" }
            );

            // ==========================================
            // 5. SEED FORM-LEVEL VISIBILITY
            // ==========================================
            modelBuilder.Entity<FormRolePermission>().HasData(
                new FormRolePermission { PermissionId = 1, FormId = 1, RoleId = 1, IsVisible = true }, // Admin
                new FormRolePermission { PermissionId = 2, FormId = 1, RoleId = 2, IsVisible = true }, // Manager
                new FormRolePermission { PermissionId = 3, FormId = 1, RoleId = 3, IsVisible = true }  // User
            );

            // ==========================================
            // 6. SEED FIELD-LEVEL GRANULAR PERMISSIONS (RBAC)
            // ==========================================
            modelBuilder.Entity<FieldRolePermission>().HasData(
                // ---- ADMIN: Can write (edit) every single field ----
                new FieldRolePermission { FieldPermissionId = 1, FieldId = 1, RoleId = 1, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 2, FieldId = 2, RoleId = 1, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 3, FieldId = 3, RoleId = 1, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 4, FieldId = 4, RoleId = 1, PermissionLevel = "Write" },

                // ---- MANAGER: Can write basic fields, but can only READ salary ----
                new FieldRolePermission { FieldPermissionId = 5, FieldId = 1, RoleId = 2, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 6, FieldId = 2, RoleId = 2, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 7, FieldId = 3, RoleId = 2, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 8, FieldId = 4, RoleId = 2, PermissionLevel = "Read" },

                // ---- USER: Can write basic fields, but salary is completely HIDDEN ----
                new FieldRolePermission { FieldPermissionId = 9, FieldId = 1, RoleId = 3, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 10, FieldId = 2, RoleId = 3, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 11, FieldId = 3, RoleId = 3, PermissionLevel = "Write" },
                new FieldRolePermission { FieldPermissionId = 12, FieldId = 4, RoleId = 3, PermissionLevel = "Hidden" }
            );
        }
    }
}