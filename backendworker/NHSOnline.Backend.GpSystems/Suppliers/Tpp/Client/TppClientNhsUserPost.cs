using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientNhsUserPost : ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientNhsUserPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<AddNhsUserResponse>> Post(AddNhsUserRequest addNhsUserRequest)
        {
            var response = await _requestExecutor.Post<AddNhsUserResponse>(
                requestBuilder => requestBuilder.Model(addNhsUserRequest));

            if (response.Body != null)
            {
                response.Body.ProviderId = addNhsUserRequest.Application.ProviderId;
            }

            return response;
        }
    }
}