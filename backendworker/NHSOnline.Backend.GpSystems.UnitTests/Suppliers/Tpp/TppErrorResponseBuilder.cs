using System;
using System.Xml;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    internal sealed class TppErrorResponseBuilder
    {
        private string _errorCode = "ERR";
        private string _userFriendlyMessage = "Friendly Message";
        private string _technicalMessage = "Technical Message";
        private Guid _uuid = Guid.Parse("{6398EA18-0894-402C-B269-779F57DF5157}");

        internal TppErrorResponseBuilder ErrorCode(string errorCode) => Set(b => b._errorCode = errorCode);

        internal string BuildXml() => new TppXmlResponseBuilder("Error").BuildXml(WriteXml);

        private void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("errorCode", _errorCode);
            writer.WriteAttributeString("userFriendlyMessage", _userFriendlyMessage);
            writer.WriteAttributeString("technicalMessage", _technicalMessage);
            writer.WriteAttributeString("uuid", _uuid.ToString());
        }

        internal Error BuildExpected()
            => new Error
            {
                ErrorCode = _errorCode,
                TechnicalMessage = _technicalMessage,
                UserFriendlyMessage = _userFriendlyMessage,
                Uuid = _uuid
            };

        private TppErrorResponseBuilder Set(Action<TppErrorResponseBuilder> setter)
        {
            setter(this);
            return this;
        }
    }
}