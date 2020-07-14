namespace NHSOnline.App.Api.Client.Errors
{
    internal sealed class PfsErrorResponseValidator : IResponseModelValidator<PfsErrorResponseModel, PfsErrorResponse>
    {
        public ModelValidationResult<PfsErrorResponse> Validate(PfsErrorResponseModel model)
        {
            if (model.ServiceDeskReference != null)
            {
                var response = new PfsErrorResponse(model.ServiceDeskReference);
                return new ModelValidationResult<PfsErrorResponse>.Valid(response);
            }

            return new ModelValidationResult<PfsErrorResponse>.Invalid();
        }
    }
}