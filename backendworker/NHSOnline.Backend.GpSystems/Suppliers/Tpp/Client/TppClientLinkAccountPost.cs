using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientLinkAccountPost : ITppClientRequest<LinkAccount, LinkAccountReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientLinkAccountPost(TppClientRequestExecutor requestExecutor) => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<LinkAccountReply>> Post(LinkAccount linkAccountModel)
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