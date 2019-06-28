namespace NHSOnline.Backend.Support
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}