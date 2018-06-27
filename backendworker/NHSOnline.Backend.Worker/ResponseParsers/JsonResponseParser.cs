namespace NHSOnline.Backend.Worker.ResponseParsers
{
    public class JsonResponseParser: BaseResponseParser, IJsonResponseParser
    {
        protected override T Deserialize<T>(string body)
        {
            return body.DeserializeJson<T>();
        }
    }
}
