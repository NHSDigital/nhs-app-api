using NHSOnline.Backend.Worker.Areas.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public abstract class GetLinkageResult
    {
        public abstract T Accept<T>(IGetLinkageResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetLinkageResult
        {
            public LinkageResponse Response { get; }

            public SuccessfullyRetrieved(LinkageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IGetLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetLinkageResult
        {
            public override T Accept<T>(IGetLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NhsNumberNotFound : GetLinkageResult
        {
            public override T Accept<T>(IGetLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : GetLinkageResult
        {
            public override T Accept<T>(IGetLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }   
                
        public class LinkageKeyRevoked : GetLinkageResult
        {
            public override T Accept<T>(IGetLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
