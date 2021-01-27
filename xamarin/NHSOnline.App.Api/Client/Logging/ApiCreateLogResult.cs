namespace NHSOnline.App.Api.Logging
{
    public abstract class ApiCreateLogResult
    {
        private ApiCreateLogResult() {}

        public abstract T Accept<T>(ICreateLogResultVisitor<T> visitor);

        public sealed class Created : ApiCreateLogResult
        {
            public override T Accept<T>(ICreateLogResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failure : ApiCreateLogResult
        {
            public override T Accept<T>(ICreateLogResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}