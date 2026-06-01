namespace DynamicForms_WebAPI.Models
{
    // 1. Used to capture login credentials from the React login page
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // 2. Used to return the successful login payload along with the security token
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

    // 3. The precise structure sent to React to build the dynamic inputs in the UI
    public class FormFieldDto
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty; // Text, Number, Date, etc.
        public bool IsRequired { get; set; }
        public string? ValidationRules { get; set; } // The raw JSON string containing custom rules
        public string PermissionLevel { get; set; } = string.Empty;
    }

    // 4. Used to capture dynamic responses when a user clicks 'Submit' on a form
    public class SubmissionRequest
    {
        public int FormId { get; set; }
        public string JsonData { get; set; } = string.Empty; 
    }
}