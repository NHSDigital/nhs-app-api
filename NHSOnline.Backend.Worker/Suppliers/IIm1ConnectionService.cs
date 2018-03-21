using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Models.Patient;

namespace NHSOnline.Backend.Worker.Suppliers
{
    public interface IIm1ConnectionService
    {
        Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode);
        Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request);
    }

    public abstract class Im1ConnectionVerifyResult
    {
        private Im1ConnectionVerifyResult()
        {
        }

        public abstract T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor);

        public class SuccessfullyVerified : Im1ConnectionVerifyResult
        {
            public PatientIm1ConnectionResponse Response { get; }

            public SuccessfullyVerified(PatientIm1ConnectionResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InsufficientPermissions : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public interface IIm1ConnectionVerifyResultVisitor<T>
        {
            T Visit(SuccessfullyVerified result);
            T Visit(InsufficientPermissions result);
            T Visit(NotFound result);
            T Visit(SupplierSystemUnavailable result);
        }
    }

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

        public interface IIm1ConnectionRegisterResultVisitor<T>
        {
            T Visit(SuccessfullyRegistered result);
            T Visit(InsufficientPermissions result);
            T Visit(NotFound result);
            T Visit(AccountAlreadyExists result);
            T Visit(SupplierSystemUnavailable result);
        }
    }
}