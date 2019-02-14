namespace NHSOnline.Backend.Support
{
    public interface IEnumMapper<TFrom, TEnum>
        where TEnum : struct
    {
        TEnum To(TFrom source);

        TFrom From(TEnum source);
    }
}