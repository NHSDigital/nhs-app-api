using System.Collections.Generic;
using System.Net;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy
{
    public abstract class PharmacySearchResult
    {
        public abstract T Accept<T>(IPharmacySearchResponseVisitor<T> visitor);

        public class Success : PharmacySearchResult
        {
            public PharmacySearchResultResponse Response { get; }
            public Success(IEnumerable<PharmacyDetails> pharmacies, int? pharmacyCount = null)
            {
                Response = new PharmacySearchResultResponse
                {
                    Pharmacies = pharmacies,
                    PharmacyCount = pharmacyCount,
                };
            }
            
            public override T Accept<T>(IPharmacySearchResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InvalidPostcode : PharmacySearchResult
        {
            public override T Accept<T>(IPharmacySearchResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class PostcodeResultFailure : PharmacySearchResult
        {
            public HttpStatusCode StatusCode { get; }

            public PostcodeResultFailure(HttpStatusCode statusCode)
            {
                StatusCode = statusCode;
            }
            
            public override T Accept<T>(IPharmacySearchResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        
        public class BadRequest : PharmacySearchResult
        {
            public override T Accept<T>(IPharmacySearchResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : PharmacySearchResult
        {      
            public override T Accept<T>(IPharmacySearchResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class ModelValidationError : PharmacySearchResult
        {      
            public override T Accept<T>(IPharmacySearchResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class UnsafeSearchTerm : PharmacySearchResult
        {
            public override T Accept<T>(IPharmacySearchResponseVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}