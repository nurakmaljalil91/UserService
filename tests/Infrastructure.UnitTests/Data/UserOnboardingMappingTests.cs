using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.UnitTests.Data;

/// <summary>
/// Verifies that registration's pending status is included in relational inserts.
/// </summary>
public class UserOnboardingMappingTests
{
    [Fact]
    public void PendingStatus_IsNotTreatedAsAnUnsetDatabaseDefault()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=unused;Username=unused;Password=unused",
                builder => builder.UseNodaTime())
            .UseSnakeCaseNamingConvention()
            .Options;
        using var context = new ApplicationDbContext(options);

        var property = context.Model.FindEntityType(typeof(User))!
            .FindProperty(nameof(User.OnboardingStatus))!;

        Assert.Equal(ValueGenerated.OnAdd, property.ValueGenerated);
        Assert.NotEqual(UserOnboardingStatus.Pending, property.Sentinel);
        Assert.Equal(UserOnboardingStatus.Completed, property.GetDefaultValue());
    }
}
