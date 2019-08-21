namespace NHSOnline.Backend.Support
{
    public interface IApiErrorResponse
    {        
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
    }
}