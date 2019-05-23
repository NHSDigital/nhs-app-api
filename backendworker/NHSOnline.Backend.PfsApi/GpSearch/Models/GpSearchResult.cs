namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public abstract class GpSearchResult
    {
        public abstract T Accept<T>(IGpSearchResultVisitor<T> visitor);

        public class Success : GpSearchResult
        {
            public GpSearchResponse Response { get; }

            public Success(GpSearchResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IGpSearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : GpSearchResult
        {
            public override T Accept<T>(IGpSearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GpSearchResult
        {
            public override T Accept<T>(IGpSearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : GpSearchResult
        {
            public override T Accept<T>(IGpSearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}