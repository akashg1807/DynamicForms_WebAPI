using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DynamicForms_WebAPI.Data;
using DynamicForms_WebAPI.Models;

namespace DynamicForms_WebAPI.Controllers
{
    [Authorize] // Requires a valid JWT token.
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FormController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // Endpoint 1: Fetch dynamic fields filtered by user role
        // ==========================================================
        [HttpGet("{formId}")]
        public async Task<IActionResult> GetFormMetadata(int formId)
        {
            // 1. Extract the Role ID claim from the current user context
            var roleIdClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleIdClaim)) return Unauthorized();
            int roleId = int.Parse(roleIdClaim);

            // 2. Enforce Form-Level Access: Can this role see this form at all?
            var formPermission = await _context.FormRolePermissions
                .FirstOrDefaultAsync(p => p.FormId == formId && p.RoleId == roleId);

            if (formPermission == null || !formPermission.IsVisible)
            {
                return Forbid(); // Returns 403 Forbidden if layout metadata is locked out for this role
            }

            // 3. Query the layout schema and filter out any fields classified as 'Hidden'
            var allowedFields = await _context.FormFields
                .Where(f => f.FormId == formId)
                .SelectMany(f => f.FieldRolePermissions
                    .Where(p => p.RoleId == roleId && p.PermissionLevel != "Hidden")
                    .Select(p => new FormFieldDto
                    {
                        FieldId = f.FieldId,
                        FieldName = f.FieldName,
                        FieldType = f.FieldType,
                        IsRequired = f.IsRequired,
                        ValidationRules = f.ValidationRules,
                        PermissionLevel = p.PermissionLevel // 'Write' or 'Read'
                    }))
                .ToListAsync();

            return Ok(allowedFields);
        }

        // ==========================================================
        // Endpoint 2: Store dynamic responses as structured JSON
        // POST: api/form/submit
        // ==========================================================
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitForm([FromBody] SubmissionRequest request)
        {
            // 1. Safely extract the UserId from the authenticated token claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            // 2. Map the request to a new FormSubmission entity
            var submission = new FormSubmission
            {
                FormId = request.FormId,
                SubmittedBy = userId,
                ResponseData = request.JsonData,
                SubmittedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 3. Persist the response inside MSSQL
            _context.FormSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Form submission successfully preserved!" });
        }
    }
}