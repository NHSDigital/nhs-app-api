using NHSOnline.Backend.PfsApi.Areas.OdsCode.Models;

namespace NHSOnline.Backend.PfsApi.Areas.OdsCode
{
    public abstract class GetOdsCodeLookupResult
    {
        private GetOdsCodeLookupResult()
        {
        }

        public abstract T Accept<T>(IGetOdsCodeLookupResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetOdsCodeLookupResult
        {
            public SuccessfullyRetrieved(bool isGpSystemSupported)
            {
                Response = new GetOdsCodeLookupResponse
                {
                    IsGpSystemSupported = isGpSystemSupported,
                };
            }

            public GetOdsCodeLookupResponse Response { get; set; }

            public override T Accept<T>(IGetOdsCodeLookupResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorRetrievingOdsCode : GetOdsCodeLookupResult
        {
            public override T Accept<T>(IGetOdsCodeLookupResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
