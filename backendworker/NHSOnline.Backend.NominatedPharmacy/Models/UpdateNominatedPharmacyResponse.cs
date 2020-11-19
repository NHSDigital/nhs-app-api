using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public abstract class UpdateNominatedPharmacyResponse
    {
        public abstract T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor);
        
        public class SuccessfullyUpdated : UpdateNominatedPharmacyResponse
        {
            public string OldOdsCode { get; }
            public string NewOdsCode { get; }

            public SuccessfullyUpdated(string oldOdsCode, string newOdsCode)
            {
                OldOdsCode = oldOdsCode;
                NewOdsCode = newOdsCode;
            }

            public override T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SuccessfullyCreated : UpdateNominatedPharmacyResponse
        {
            public string NewOdsCode { get; }
            
            public SuccessfullyCreated(string newOdsCode)
            {
                NewOdsCode = newOdsCode;
            }
            
            public override T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class GetNominatedPharmacyFailure : UpdateNominatedPharmacyResponse
        {
            public StatusCodeResult StatusCode { get; }

            public GetNominatedPharmacyFailure(StatusCodeResult statusCode)
            {
                StatusCode = statusCode;
            }
            
            public override T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : UpdateNominatedPharmacyResponse
        {    
            public HttpStatusCode StatusCode { get; }
            
            public InternalServerError(HttpStatusCode statusCode)
            {
                StatusCode = statusCode;
            }
            
            public override T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : UpdateNominatedPharmacyResponse
        {   public string OldOdsCode { get; }
            public string NewOdsCode { get; }
            
            public BadGateway(string oldOdsCode, string newOdsCode)
            {
                OldOdsCode = oldOdsCode;
                NewOdsCode = newOdsCode;
            }  
            
            public override T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class UpdatedButStillOldCode : UpdateNominatedPharmacyResponse
        {
            public string OldOdsCode { get; }
            public string NewOdsCode { get; }
            
            public UpdatedButStillOldCode(string oldOdsCode, string newOdsCode)
            {
                OldOdsCode = oldOdsCode;
                NewOdsCode = newOdsCode;
            }
            
            public override T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class NominatedPharmacyIsDisabled : UpdateNominatedPharmacyResponse
        {    
            public override T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : UpdateNominatedPharmacyResponse
        {
            public override T Accept<T>(IUpdateNominatedPharmacyResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }     
    }
}