namespace NHSOnline.Backend.Support.Certificate
{
    public interface IKeyConfig
    {
        string KeyPath { get; }
        string Password { get; }
    }
}