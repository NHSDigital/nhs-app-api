using NHSOnline.Backend.Worker.Areas.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public abstract class CreateLinkageResult
    {
        public abstract T Accept<T>(ICreateLinkageResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : CreateLinkageResult
        {
            public LinkageResponse Response { get; }

            public SuccessfullyRetrieved(LinkageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ICreateLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NhsNumberNotFound : CreateLinkageResult
        {
            public override T Accept<T>(ICreateLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class LinkageKeyAlreadyExists : CreateLinkageResult
        {
            public override T Accept<T>(ICreateLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : CreateLinkageResult
        {
            public override T Accept<T>(ICreateLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierSystemUnavailable : CreateLinkageResult
        {
            public override T Accept<T>(ICreateLinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
