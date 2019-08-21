using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public abstract class OrderPrescriptionResult
    {   
        public abstract T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor);
        
        public class Success : OrderPrescriptionResult
        {
            public override T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : OrderPrescriptionResult
        {
            public override T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : OrderPrescriptionResult
        {
            public override T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : OrderPrescriptionResult
        {
            public override T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : OrderPrescriptionResult
        {
            public override T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class CannotReorderPrescription : OrderPrescriptionResult
        {
            public override T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class MedicationAlreadyOrderedWithinLast30Days : OrderPrescriptionResult
        {
            public override T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class PartialSuccess : OrderPrescriptionResult
        {
            public PrescriptionRequestPostPartialSuccessResponse Response { get; }

            public PartialSuccess(PrescriptionRequestPostPartialSuccessResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IOrderPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}