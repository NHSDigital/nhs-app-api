namespace NHSOnline.App.Api.Client.Session
{
    internal abstract class ApiCreateSessionResult
    {
        private ApiCreateSessionResult() { }

        internal abstract T Accept<T>(IApiCreateSessionResultVisitor<T> visitor);

        internal sealed class Success: ApiCreateSessionResult
        {
            internal Success(ApiCreateSessionResponse response) => Response = response;

            internal ApiCreateSessionResponse Response { get; }

            internal override T Accept<T>(IApiCreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Failure : ApiCreateSessionResult
        {
            internal override T Accept<T>(IApiCreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}