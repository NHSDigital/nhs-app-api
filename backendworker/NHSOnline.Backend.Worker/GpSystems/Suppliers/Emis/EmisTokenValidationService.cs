using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisTokenValidationService: ITokenValidationService
    {
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {
            return Guid.TryParse(connectionToken, out var _);
        }
    }
}
