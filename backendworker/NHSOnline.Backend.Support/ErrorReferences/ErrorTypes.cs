namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        private ErrorTypes()
        {
        }  

        public abstract string Prefix { get; }
        
        public abstract ErrorCategory Category { get; }

        public abstract int StatusCode { get; }

        public virtual SourceApi SourceApi => SourceApi.None;
    }
}