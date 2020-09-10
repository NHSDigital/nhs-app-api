namespace NHSOnline.Backend.PfsApi.GpSession
{
    public interface IGpSessionRecreateResultVisitor<T>
    {
        public T Visit(GpSessionRecreateResult.RecreatedResult createRecreatedResult);
        public T Visit(GpSessionRecreateResult.SessionStillValidResult createRecreatedResult);
        public T Visit(GpSessionRecreateResult.ErrorResult errorResult);
    }
}
