using System;
using Microsoft.Azure.Documents.SystemFunctions;
using NHSOnline.Backend.Worker.GpSystems.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisRegistrationGuidKeyGenerator : IRegistrationGuidKeyGenerator
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