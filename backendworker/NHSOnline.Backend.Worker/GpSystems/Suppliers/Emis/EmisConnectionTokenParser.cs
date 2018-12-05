using System;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
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
