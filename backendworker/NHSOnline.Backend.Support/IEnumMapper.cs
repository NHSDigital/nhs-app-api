using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.Support
{
    public interface IEnumMapper<TFrom, TEnum>
        where TEnum : struct
    {
        [SuppressMessage("Microsoft.Naming", "CA1716", 
            Justification = "We knowingly choose to use the 'To' keyword. This class library will not be consumed externally.")]
        TEnum To(TFrom source);

        TFrom From(TEnum source);
    }
}