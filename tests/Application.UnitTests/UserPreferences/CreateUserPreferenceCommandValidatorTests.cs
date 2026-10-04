#nullable enable
using Application.UserPreferences;
using Application.UserPreferences.Commands;

namespace Application.UnitTests.UserPreferences;

/// <summary>
/// Unit tests for <see cref="CreateUserPreferenceCommandValidator"/> and <see cref="TimeZonePreference"/>.
/// </summary>
public class CreateUserPreferenceCommandValidatorTests
{
    private readonly CreateUserPreferenceCommandValidator _validator = new();

    /// <summary>
    /// Ensures valid IANA time zone ids are accepted for the time zone key.
    /// </summary>
    /// <param name="value">The time zone value.</param>
    [Theory]
    [InlineData("Asia/Kuala_Lumpur")]
    [InlineData("Asia/Singapore")]
    [InlineData("UTC")]
    public void Validate_WhenTimeZoneValid_IsValid(string value)
    {
        var result = _validator.Validate(new CreateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            Key = TimeZonePreference.Key,
            Value = value
        });

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Ensures offsets, unknown ids, and empty values are rejected for the time zone key.
    /// </summary>
    /// <param name="value">The time zone value.</param>
    [Theory]
    [InlineData("UTC+7")]
    [InlineData("Mars/Olympus")]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenTimeZoneInvalid_HasValueError(string? value)
    {
        var result = _validator.Validate(new CreateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            Key = TimeZonePreference.Key,
            Value = value
        });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateUserPreferenceCommand.Value)
                                            && e.ErrorMessage == TimeZonePreference.InvalidValueMessage);
    }

    /// <summary>
    /// Ensures values of other keys are not validated as time zones.
    /// </summary>
    [Fact]
    public void Validate_WhenOtherKey_DoesNotValidateTimeZone()
    {
        var result = _validator.Validate(new CreateUserPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            Key = "ui.theme",
            Value = "UTC+7"
        });

        Assert.True(result.IsValid);
    }
}
