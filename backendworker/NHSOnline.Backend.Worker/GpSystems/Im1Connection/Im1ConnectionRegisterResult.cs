using NHSOnline.Backend.Worker.Areas.Im1Connection;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Im1Connection
{
    public abstract class Im1ConnectionRegisterResult
    {
        private Im1ConnectionRegisterResult()
        {
        }

        public abstract T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor);

        public class SuccessfullyRegistered : Im1ConnectionRegisterResult
        {
            public PatientIm1ConnectionResponse Response { get; }

            public SuccessfullyRegistered(PatientIm1ConnectionResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InsufficientPermissions : Im1ConnectionRegisterResult
        {
            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : Im1ConnectionRegisterResult
        {
            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class AccountAlreadyExists : Im1ConnectionRegisterResult
        {
            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : Im1ConnectionRegisterResult
        {
            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : Im1ConnectionRegisterResult
        {
            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}