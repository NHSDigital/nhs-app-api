namespace NHSOnline.Backend.Support.Middleware
{
    public class LogRequestHeaderOptions
    {
        
        public const string NameText = "{name}";
        public const string ValueText = "{value}";
        
        /// <summary>
        /// The name of the header ex- CorrelationId.
        /// </summary>
        public string HeaderName { get; set; }

        /// <summary>
        /// Controls whether the format of Log text.
        /// <para>Example: {name}:{value}</para>
        /// </summary>
        public string LogTemplate { get; set; } = $"{NameText}={ValueText}";
    }
}