namespace NHSOnline.Backend.Support.Hasher
{
    public interface ISha512ProviderFactory
    {
        ISha512Provider Build();
    }

    public class Sha512ProviderFactory : ISha512ProviderFactory
    {
        public ISha512Provider Build() => new Sha512Provider();
    }
}
