namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public interface IOnlineConsultationsProviderHttpClientPool
    {
        IOnlineConsultationsProviderHttpClient GetClientByProviderName(string provider);
    }
}