using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions
{
    public abstract class PrescriptionResult
    {   
        public abstract T Accept<T>(IPrescriptionResultVisitor<T> visitor);

        public class SuccessfullGet : PrescriptionResult
        {
            public PrescriptionListResponse Response { get; }

            public SuccessfullGet(PrescriptionListResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SuccessfullPost : PrescriptionResult
        {
            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : PrescriptionResult
        {
            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierNotEnabled : PrescriptionResult
        {
            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : PrescriptionResult
        {
            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : PrescriptionResult
        {
            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class CannotReorderPrescription : PrescriptionResult
        {
            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}