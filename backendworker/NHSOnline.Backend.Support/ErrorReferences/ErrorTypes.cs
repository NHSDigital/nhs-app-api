using Microsoft.AspNetCore.Http;

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

        public class UnhandledError : ErrorTypes
        {
            public override string Prefix => "xx";
            public override ErrorCategory Category => ErrorCategory.None;
            public override int StatusCode => StatusCodes.Status500InternalServerError;
        }
    }
}