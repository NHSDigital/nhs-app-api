namespace NHSOnline.App.Api.Client
{
    internal interface IModelValidationResultVisitor<TResponse, TResult>
    {
        TResult Visit(ModelValidationResult<TResponse>.Valid valid);
        TResult Visit(ModelValidationResult<TResponse>.Invalid invalid);
    }
}