namespace NHSOnline.App.Api.Client
{
    internal interface IResponseModelValidator<TResponseModel, TResponse>
    {
        TResponse Validate(TResponseModel model);
    }
}