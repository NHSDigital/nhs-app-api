using System.Net;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public abstract class GetNominatedPharmacyResult
    {
        public abstract T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor);

        public abstract bool IsSuccess();
        
        public GetNominatedPharmacyResponse GetNominatedPharmacyResponse { get; set; }

        public class Success : GetNominatedPharmacyResult
        {
            public PharmacyDetails PharmacyDetails { get; set; }
            
            public Success(GetNominatedPharmacyResponse getNominatedPharmacyResponse)
            {
                GetNominatedPharmacyResponse = getNominatedPharmacyResponse;
            }

            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

            public override bool IsSuccess()
            {
                return true;
            }
        }


        public class PharmacyRetrievalFailure : GetNominatedPharmacyResult
        {
            public PharmacyRetrievalFailure(HttpStatusCode statusCode)
            {
                GetNominatedPharmacyResponse = new GetNominatedPharmacyResponse(statusCode, false);
            }
            
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
            
            public override bool IsSuccess()
            {
                return false;
            }
        }

        public class PersonalChecksFailed : GetNominatedPharmacyResult
        { 
            public PersonalChecksFailed(GetNominatedPharmacyResponse getNominatedPharmacyResponse)
            {
                GetNominatedPharmacyResponse = getNominatedPharmacyResponse;
            }
            
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        public class PharmacyChecksFailed : GetNominatedPharmacyResult
        {
            public PharmacyChecksFailed(HttpStatusCode statusCode)
            {
                GetNominatedPharmacyResponse = new GetNominatedPharmacyResponse(statusCode, false);
            }
            
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            } 
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        public class NhsNumberSuperseded : GetNominatedPharmacyResult
        {
            public string SentNhsNumber { get; set; }
            public string ReturnedNhsNumber { get; set; }
            
            public NhsNumberSuperseded(HttpStatusCode statusCode, string sentNhsNumber, string returnedNhsNumber)
            {
                GetNominatedPharmacyResponse = new GetNominatedPharmacyResponse(statusCode, false);
                SentNhsNumber = sentNhsNumber;
                ReturnedNhsNumber = returnedNhsNumber;
            }
            
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            } 
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        public class ConfidentialAccount : GetNominatedPharmacyResult
        {
            public ConfidentialAccount(HttpStatusCode statusCode)
            {
                GetNominatedPharmacyResponse = new GetNominatedPharmacyResponse(statusCode, false);
            }
            
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            } 
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        public class InternalServerError : GetNominatedPharmacyResult
        {
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            } 
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        public class ConfigNotEnabled : GetNominatedPharmacyResult
        {
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }  
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        public class GpPracticeEpsNotEnabled : GetNominatedPharmacyResult
        {
            public string OdsCode { get; set; }

            public GpPracticeEpsNotEnabled(string odsCode)
            {
                OdsCode = odsCode;
            }
            
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }  
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        
        public class GpPracticeFailure : GetNominatedPharmacyResult
        {
            public string OdsCode { get; set; }
            public HttpStatusCode StatusCode { get; set; }

            public GpPracticeFailure(string odsCode, HttpStatusCode statusCode)
            {
                OdsCode = odsCode;
                StatusCode = statusCode;
            }
            
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            } 
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
       
        public class NoNominatedPharmacy : GetNominatedPharmacyResult
        {    
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        public class PharmacyDetailFailure : GetNominatedPharmacyResult
        {
            public string OdsCode { get; set; }
            public HttpStatusCode StatusCode { get; set; }

            public PharmacyDetailFailure(string odsCode, HttpStatusCode statusCode)
            {
                OdsCode = odsCode;
                StatusCode = statusCode;
            }
            
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            } 
            
            public override bool IsSuccess()
            {
                return false;
            }
        }       
        
        public class InvalidPharmacySubtype : GetNominatedPharmacyResult
        {
            public override T Accept<T>(IGetNominatedPharmacyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            } 
            
            public override bool IsSuccess()
            {
                return false;
            }
        }
        
        public static int GetErrorStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.InternalServerError
                ? (int) statusCode
                : (int) HttpStatusCode.BadGateway;                                                                             
        }                                                                                                          

    }
}