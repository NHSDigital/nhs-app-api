namespace NHSOnline.Backend.ClinicalDecisionSupportApi.HttpClients
{
    public interface IOnlineConsultationsProviderHttpClientPool
    {
        IOnlineConsultationsProviderHttpClient GetClientByProviderName(string provider);
    }
}