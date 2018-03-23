using System;
using NHSOnline.Backend.Worker.Router;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisTokenValidationService: ITokenValidationService
    {
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {
            return Guid.TryParse(connectionToken, out var result);
        }
    }
}
