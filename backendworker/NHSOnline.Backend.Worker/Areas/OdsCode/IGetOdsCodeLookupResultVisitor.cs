namespace NHSOnline.Backend.Worker.Areas.OdsCode
{
    public interface IGetOdsCodeLookupResultVisitor<out T>
    {
        T Visit(GetOdsCodeLookupResult.SuccessfullyRetrieved result);

        T Visit(GetOdsCodeLookupResult.ErrorRetrievingOdsCode result);       
    }
}