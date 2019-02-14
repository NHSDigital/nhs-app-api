using System;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public static class EmisConnectionTokenParser
    {
        public static Either<string, EmisConnectionToken> Parse(string connectionToken)
        {
            return Guid.TryParse(connectionToken, out _)
                ? new Either<string, EmisConnectionToken>(connectionToken)
                : new Either<string, EmisConnectionToken>(connectionToken.DeserializeJson<EmisConnectionToken>());
        }
    }
}
