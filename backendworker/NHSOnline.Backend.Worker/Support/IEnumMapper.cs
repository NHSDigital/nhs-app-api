namespace NHSOnline.Backend.Worker.Support
{
    internal interface IEnumMapper<TFrom, TEnum>
        where TEnum : struct
    {
        TEnum To(TFrom source);

        TFrom From(TEnum source);
    }
}