using System;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppTokenValidationService : ITokenValidationService
    {
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {   
            if (string.IsNullOrEmpty(connectionToken))
            {
                return false;
            }
            
            try
            {
                var auth = connectionToken.DeserializeJson<Authenticate>();
                return !string.IsNullOrEmpty(auth?.AccountId) && !string.IsNullOrEmpty(auth.Passphrase);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}