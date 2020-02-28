namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    internal interface ITppLogMessagingService
    {
        void FetchAndLogAccessInformation(TppUserSession userSession);
    }
}