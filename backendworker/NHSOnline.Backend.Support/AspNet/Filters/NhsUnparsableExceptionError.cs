namespace NHSOnline.Backend.Support.AspNet.Filters
{
    public class NhsUnparsableExceptionError
    {
        public NhsUnparsableExceptionError(string message, string path)
        {
            Message = message;
            Path = path;
        }

        public string Message { get; }
        public string Path { get; }

        public override string ToString()
        {
            return $"Error: '{Message}' at path '{Path}'.";
        }
    }
}