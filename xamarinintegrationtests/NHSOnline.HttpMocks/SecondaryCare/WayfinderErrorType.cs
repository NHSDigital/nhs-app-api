namespace NHSOnline.HttpMocks.SecondaryCare
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// The lowercase naming is required for serialization, as it currently
    /// does not support conversion from `PascalCase` to `camelCase`.
    /// </summary>
    public enum WayfinderErrorType
    {
        none,
        underSixteen,
        generalError,
    }
}