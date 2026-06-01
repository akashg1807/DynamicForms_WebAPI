using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForms_WebAPI.Data
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty; // Admin, Manager, User

        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;
    }

    public class Form
    {
        [Key]
        public int FormId { get; set; }

        [Required]
        [StringLength(100)]
        public string FormName { get; set; } = string.Empty;

        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FormField> FormFields { get; set; } = new List<FormField>();
    }

    public class FormField
    {
        [Key]
        public int FieldId { get; set; }

        public int FormId { get; set; }

        [ForeignKey("FormId")]
        public Form Form { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty; 

        [Required]
        [StringLength(50)]
        public string FieldType { get; set; } = string.Empty; 

        public bool IsRequired { get; set; }


        public string? ValidationRules { get; set; }

        public ICollection<FieldRolePermission> FieldRolePermissions { get; set; } = new List<FieldRolePermission>();
    }

    public class FormRolePermission
    {
        [Key]
        public int PermissionId { get; set; }

        public int FormId { get; set; }
        public int RoleId { get; set; }
        public bool IsVisible { get; set; } 
    }

    public class FieldRolePermission
    {
        [Key]
        public int FieldPermissionId { get; set; }

        public int FieldId { get; set; }

        [ForeignKey("FieldId")]
        public FormField FormField { get; set; } = null!;

        public int RoleId { get; set; }

        [Required]
        [StringLength(20)]
        public string PermissionLevel { get; set; } = string.Empty; // Read, Write, Hidden
    }

    public class FormSubmission
    {
        [Key]
        public int SubmissionId { get; set; }

        public int FormId { get; set; }
        public int SubmittedBy { get; set; }

        [Required]
        public string ResponseData { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}