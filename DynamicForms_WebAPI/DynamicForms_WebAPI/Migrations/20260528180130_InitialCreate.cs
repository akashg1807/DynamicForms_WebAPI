using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DynamicForms_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormRolePermissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormRolePermissions", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.FormId);
                });

            migrationBuilder.CreateTable(
                name: "FormSubmissions",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    SubmittedBy = table.Column<int>(type: "int", nullable: false),
                    ResponseData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmissions", x => x.SubmissionId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "FormFields",
                columns: table => new
                {
                    FieldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ValidationRules = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormFields", x => x.FieldId);
                    table.ForeignKey(
                        name: "FK_FormFields_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldRolePermissions",
                columns: table => new
                {
                    FieldPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldRolePermissions", x => x.FieldPermissionId);
                    table.ForeignKey(
                        name: "FK_FieldRolePermissions_FormFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FormFields",
                        principalColumn: "FieldId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FormRolePermissions",
                columns: new[] { "PermissionId", "FormId", "IsVisible", "RoleId" },
                values: new object[,]
                {
                    { 1, 1, true, 1 },
                    { 2, 1, true, 2 },
                    { 3, 1, true, 3 }
                });

            migrationBuilder.InsertData(
                table: "Forms",
                columns: new[] { "FormId", "CreatedAt", "CreatedBy", "FormName" },
                values: new object[] { 1, new DateTime(2026, 5, 28, 18, 1, 29, 709, DateTimeKind.Utc).AddTicks(3769), 1, "Employee Onboarding Form" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Manager" },
                    { 3, "User" }
                });

            migrationBuilder.InsertData(
                table: "FormFields",
                columns: new[] { "FieldId", "FieldName", "FieldType", "FormId", "IsRequired", "ValidationRules" },
                values: new object[,]
                {
                    { 1, "Full Name", "Text", 1, true, "{\"minLength\": 2, \"maxLength\": 50}" },
                    { 2, "Joining Date", "Date", 1, true, null },
                    { 3, "Remote Worker", "Checkbox", 1, false, null },
                    { 4, "Approved Salary", "Number", 1, true, "{\"min\": 1000}" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "PasswordHash", "RoleId", "Username" },
                values: new object[,]
                {
                    { 1, "password123", 1, "admin_bob" },
                    { 2, "password123", 2, "manager_alice" },
                    { 3, "password123", 3, "user_charlie" }
                });

            migrationBuilder.InsertData(
                table: "FieldRolePermissions",
                columns: new[] { "FieldPermissionId", "FieldId", "PermissionLevel", "RoleId" },
                values: new object[,]
                {
                    { 1, 1, "Write", 1 },
                    { 2, 2, "Write", 1 },
                    { 3, 3, "Write", 1 },
                    { 4, 4, "Write", 1 },
                    { 5, 1, "Write", 2 },
                    { 6, 2, "Write", 2 },
                    { 7, 3, "Write", 2 },
                    { 8, 4, "Read", 2 },
                    { 9, 1, "Write", 3 },
                    { 10, 2, "Write", 3 },
                    { 11, 3, "Write", 3 },
                    { 12, 4, "Hidden", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldRolePermissions_FieldId",
                table: "FieldRolePermissions",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_FormId",
                table: "FormFields",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldRolePermissions");

            migrationBuilder.DropTable(
                name: "FormRolePermissions");

            migrationBuilder.DropTable(
                name: "FormSubmissions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "FormFields");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Forms");
        }
    }
}
