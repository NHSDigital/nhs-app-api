namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public interface IRegistrationGuidKeyGenerator
    {
        string GenerateRegistrationKey(string accountId, string odsCode, string linkageKey);
    }
}