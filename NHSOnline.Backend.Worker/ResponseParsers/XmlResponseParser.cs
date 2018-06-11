using System.IO;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.ResponseParsers
{
    public class XmlResponseParser: BaseResponseParser, IXmlResponseParser
    {
        protected override T Deserialize<T>(string body)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(new StringReader(body));
        }
    }
}