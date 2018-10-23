using System;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public class RegistrationGuidKeyGenerator : IRegistrationGuidKeyGenerator
    {
        public string GenerateRegistrationKey(string accountId, string odsCode, string linkageKey)
        {
            if (string.IsNullOrEmpty(accountId) && 
                string.IsNullOrEmpty(odsCode) && 
                string.IsNullOrEmpty(linkageKey))
            {
                throw new ArgumentException("need to provide values to create key");
            }
            return odsCode + accountId + linkageKey;
        }
    }
}