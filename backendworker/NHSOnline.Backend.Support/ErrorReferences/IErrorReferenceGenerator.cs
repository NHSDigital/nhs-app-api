namespace NHSOnline.Backend.Support
{
    public interface IErrorReferenceGenerator
    {
        string GenerateAndLogErrorReference(ErrorTypes errorTypes);

        string GenerateAndLogErrorReference(ErrorCategory category, int statusCode,
            SourceApi sourceApi = SourceApi.None);

        string GenerateAndLogErrorReference(ErrorCategory category, int statusCode, Supplier supplier);
    }
}