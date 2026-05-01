namespace Infrastructure.Messaging;

/// <summary>
/// Represents message broker configuration for MassTransit.
/// </summary>
public class MessageBrokerSettings
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "MessageBroker";

    /// <summary>
    /// Gets or sets the broker host name.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the broker virtual host.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// Gets or sets the broker user name.
    /// </summary>
    public string Username { get; set; } = "guest";

    /// <summary>
    /// Gets or sets the broker password.
    /// </summary>
    public string Password { get; set; } = "guest";
}
