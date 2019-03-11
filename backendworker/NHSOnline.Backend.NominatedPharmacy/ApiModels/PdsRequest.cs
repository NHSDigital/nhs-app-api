namespace NHSOnline.Backend.NominatedPharmacy.ApiModels
{
    public class PdsRequest<T>
    {
        // Empty constructor for serialization.
        public PdsRequest() { }

        public PdsRequest(T body)
        {
            Body = body;
        }

        public T Body { get; set; }
    }
}
