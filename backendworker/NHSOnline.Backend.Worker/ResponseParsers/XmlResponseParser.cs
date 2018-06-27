namespace NHSOnline.Backend.Worker.ResponseParsers
{
    public class XmlResponseParser : BaseResponseParser, IXmlResponseParser
    {
        protected override T Deserialize<T>(string body)
        {
            return body.DeserializeXml<T>();
        }
    }
}
