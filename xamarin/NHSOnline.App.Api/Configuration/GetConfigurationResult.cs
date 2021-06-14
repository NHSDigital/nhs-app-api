namespace NHSOnline.App.Api.Configuration
{
    public abstract class GetConfigurationResult
    {
        private GetConfigurationResult() {}

        public abstract T Accept<T>(IGetConfigurationResultVisitor<T> visitor);

        public sealed class Success : GetConfigurationResult
        {
            public VersionConfiguration VersionConfiguration { get; }

            public Success(VersionConfiguration versionConfiguration)
            {
                VersionConfiguration = versionConfiguration;
            }
            
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class BadRequest : GetConfigurationResult
        {
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failed : GetConfigurationResult
        {
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}