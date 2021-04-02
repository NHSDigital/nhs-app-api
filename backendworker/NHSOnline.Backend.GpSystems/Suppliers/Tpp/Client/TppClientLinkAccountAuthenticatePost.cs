using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientLinkAccountAuthenticatePost : ITppClientRequest<LinkAccountAuthenticate, LinkAccountReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientLinkAccountAuthenticatePost(TppClientRequestExecutor requestExecutor) => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<LinkAccountReply>> Post(LinkAccountAuthenticate linkAccountModel)
        {
            var response = await _requestExecutor.Post<LinkAccountReply>(requestBuilder => requestBuilder.Model(linkAccountModel));

            if (response.Body != null)
            {
                response.Body.ProviderId = linkAccountModel.Application.ProviderId;
            }

            return response;
        }
    }
}