namespace NHSOnline.App.Api.Client
{
    internal interface IResponseModelValidator<TResponseModel, TResponse>
    {
        ModelValidationResult<TResponse> Validate(TResponseModel model);
    }
}