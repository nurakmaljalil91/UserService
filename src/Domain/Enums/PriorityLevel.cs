using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums;

/// <summary>
/// Specifies the priority level for an item.
/// </summary>
public enum PriorityLevel
{
    /// <summary>
    /// No priority specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// Low priority.
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium priority.
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High priority.
    /// </summary>
    High = 3
}
