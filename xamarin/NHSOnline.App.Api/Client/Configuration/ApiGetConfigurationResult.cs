namespace NHSOnline.App.Api.Client.Configuration
{
    internal abstract class ApiGetConfigurationResult
    {
        private ApiGetConfigurationResult() { }

        internal abstract T Accept<T>(IApiGetConfigurationResultVisitor<T> visitor);

        internal sealed class Success: ApiGetConfigurationResult
        {
            internal Success(GetConfigurationResponse getConfigurationResponse)
            {
                GetConfigurationResponse = getConfigurationResponse;
            }

            internal GetConfigurationResponse GetConfigurationResponse { get; }

            internal override T Accept<T>(IApiGetConfigurationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Failure : ApiGetConfigurationResult
        {
            internal override T Accept<T>(IApiGetConfigurationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class BadRequest : ApiGetConfigurationResult
        {
            internal override T Accept<T>(IApiGetConfigurationResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}