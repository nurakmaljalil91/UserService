using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Infrastructure.Data;

namespace Infrastructure.Persistence.Migrations;

/// <summary>
/// Adds a server-owned onboarding state; all existing accounts are completed.
/// </summary>
[DbContext(typeof(ApplicationDbContext))]
[Migration("202610030001_AddUserOnboardingStatus")]
public sealed class AddUserOnboardingStatus : Migration
{
    /// <summary>Adds the onboarding status to existing users.</summary>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "onboarding_status",
            table: "users",
            type: "character varying(16)",
            maxLength: 16,
            nullable: false,
            defaultValue: "Completed");
    }

    /// <summary>Removes the onboarding status.</summary>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "onboarding_status", table: "users");
    }
}
