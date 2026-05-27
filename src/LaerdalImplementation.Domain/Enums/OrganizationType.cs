namespace LaerdalImplementation.Domain.Enums;

/// <summary>
/// Classifies the kind of organization. Stored as an integer in the database
/// (0, 1, 2) to avoid magic numbers in queries and application code.
/// </summary>
public enum OrganizationType
{
    /// <summary>A top-level hospital or healthcare institution.</summary>
    Hospital = 0,

    /// <summary>A department or sub-unit within a hospital.</summary>
    Department = 1,

    /// <summary>A dedicated training centre, which may span multiple departments.</summary>
    TrainingCenter = 2
}
