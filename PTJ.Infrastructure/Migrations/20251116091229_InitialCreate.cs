using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PTJ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "jobs");

            migrationBuilder.EnsureSchema(
                name: "org");

            migrationBuilder.EnsureSchema(
                name: "seeker");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "ApplicationStatuses",
                schema: "jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployeeCount = table.Column<int>(type: "int", nullable: true),
                    FoundedYear = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Checksum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "seeker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StudentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    University = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Major = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GPA = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    YearOfStudy = table.Column<int>(type: "int", nullable: true),
                    ExpectedGraduationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResumeUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GitHubUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyRegistrationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedCompanyId = table.Column<int>(type: "int", nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRegistrationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyRegistrationRequests_Companies_ApprovedCompanyId",
                        column: x => x.ApprovedCompanyId,
                        principalSchema: "org",
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyRegistrationRequests_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyRegistrationRequests_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobPosts",
                schema: "jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Benefits = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    SalaryMin = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    SalaryMax = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    SalaryPeriod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WorkType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumberOfPositions = table.Column<int>(type: "int", nullable: true),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    ApplicationCount = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsUrgent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPosts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPosts_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuingOrganization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CredentialId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredentialUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileCertificates_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "seeker",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileEducations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    InstitutionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GPA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileEducations_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "seeker",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileExperiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrentlyWorking = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileExperiences_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "seeker",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProficiencyLevel = table.Column<int>(type: "int", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileSkills_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "seeker",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobPostId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CoverLetter = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResumeUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_ApplicationStatuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "jobs",
                        principalTable: "ApplicationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_JobPosts_JobPostId",
                        column: x => x.JobPostId,
                        principalSchema: "jobs",
                        principalTable: "JobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "seeker",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobPostSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobPostId = table.Column<int>(type: "int", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredLevel = table.Column<int>(type: "int", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostSkills_JobPosts_JobPostId",
                        column: x => x.JobPostId,
                        principalSchema: "jobs",
                        principalTable: "JobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobPostId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobShifts_JobPosts_JobPostId",
                        column: x => x.JobPostId,
                        principalSchema: "jobs",
                        principalTable: "JobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    FromStatusId = table.Column<int>(type: "int", nullable: false),
                    ToStatusId = table.Column<int>(type: "int", nullable: false),
                    ChangedBy = table.Column<int>(type: "int", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationHistories_ApplicationStatuses_FromStatusId",
                        column: x => x.FromStatusId,
                        principalSchema: "jobs",
                        principalTable: "ApplicationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationHistories_ApplicationStatuses_ToStatusId",
                        column: x => x.ToStatusId,
                        principalSchema: "jobs",
                        principalTable: "ApplicationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationHistories_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jobs",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "jobs",
                table: "ApplicationStatuses",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7620), "Application submitted", 1, false, "Pending", null },
                    { 2, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7964), "Under review", 2, false, "Reviewing", null },
                    { 3, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7966), "Shortlisted for interview", 3, false, "Shortlisted", null },
                    { 4, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7967), "Interview scheduled", 4, false, "Interviewing", null },
                    { 5, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7969), "Job offer made", 5, false, "Offered", null },
                    { 6, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7970), "Offer accepted", 6, false, "Accepted", null },
                    { 7, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7972), "Application rejected", 7, false, "Rejected", null },
                    { 8, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7973), "Application withdrawn", 8, false, "Withdrawn", null },
                    { 9, new DateTime(2025, 11, 16, 9, 12, 29, 479, DateTimeKind.Utc).AddTicks(7975), "Application expired", 9, false, "Expired", null }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 16, 9, 12, 29, 486, DateTimeKind.Utc).AddTicks(2589), "Administrator", false, "ADMIN", null },
                    { 2, new DateTime(2025, 11, 16, 9, 12, 29, 486, DateTimeKind.Utc).AddTicks(2593), "Employer/Company", false, "EMPLOYER", null },
                    { 3, new DateTime(2025, 11, 16, 9, 12, 29, 486, DateTimeKind.Utc).AddTicks(2594), "Student/Job Seeker", false, "STUDENT", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistories_ApplicationId",
                table: "ApplicationHistories",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistories_ChangedByUserId",
                table: "ApplicationHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistories_FromStatusId",
                table: "ApplicationHistories",
                column: "FromStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistories_ToStatusId",
                table: "ApplicationHistories",
                column: "ToStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobPostId",
                schema: "jobs",
                table: "Applications",
                column: "JobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobPostId_ProfileId",
                schema: "jobs",
                table: "Applications",
                columns: new[] { "JobPostId", "ProfileId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ProfileId",
                schema: "jobs",
                table: "Applications",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ReviewerId",
                schema: "jobs",
                table: "Applications",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_StatusId",
                schema: "jobs",
                table: "Applications",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatuses_Name",
                schema: "jobs",
                table: "ApplicationStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_OwnerId",
                schema: "org",
                table: "Companies",
                column: "OwnerId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TaxCode",
                schema: "org",
                table: "Companies",
                column: "TaxCode",
                unique: true,
                filter: "[TaxCode] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrationRequests_ApprovedCompanyId",
                table: "CompanyRegistrationRequests",
                column: "ApprovedCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrationRequests_RequesterId",
                table: "CompanyRegistrationRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrationRequests_ReviewerId",
                table: "CompanyRegistrationRequests",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_UploadedByUserId",
                table: "Files",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_CompanyId",
                schema: "jobs",
                table: "JobPosts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_CreatedAt",
                schema: "jobs",
                table: "JobPosts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_CreatedByUserId",
                schema: "jobs",
                table: "JobPosts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_Status",
                schema: "jobs",
                table: "JobPosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostSkills_JobPostId",
                table: "JobPostSkills",
                column: "JobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_JobShifts_JobPostId",
                table: "JobShifts",
                column: "JobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileCertificates_ProfileId",
                table: "ProfileCertificates",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileEducations_ProfileId",
                table: "ProfileEducations",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileExperiences_ProfileId",
                table: "ProfileExperiences",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                schema: "seeker",
                table: "Profiles",
                column: "UserId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkills_ProfileId",
                table: "ProfileSkills",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "auth",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "auth",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "auth",
                table: "Roles",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "auth",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "auth",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationHistories");

            migrationBuilder.DropTable(
                name: "CompanyRegistrationRequests");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "JobPostSkills");

            migrationBuilder.DropTable(
                name: "JobShifts");

            migrationBuilder.DropTable(
                name: "ProfileCertificates");

            migrationBuilder.DropTable(
                name: "ProfileEducations");

            migrationBuilder.DropTable(
                name: "ProfileExperiences");

            migrationBuilder.DropTable(
                name: "ProfileSkills");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ApplicationStatuses",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "JobPosts",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "seeker");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "org");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "auth");
        }
    }
}
