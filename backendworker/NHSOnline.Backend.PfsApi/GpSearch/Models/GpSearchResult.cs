using NHSOnline.Backend.PfsApi.GpSearch.GpLookup;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public abstract class GpSearchResult
    {
        public abstract T Accept<T>(IGpSearchResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GpSearchResult
        {
            public GpSearchResponse Response { get; }

            public SuccessfullyRetrieved(GpSearchResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IGpSearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : GpSearchResult
        {
            public override T Accept<T>(IGpSearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NhsSearchServiceUnavailable : GpSearchResult
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