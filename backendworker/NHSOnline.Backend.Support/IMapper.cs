using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.Support
{
    public interface IMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource source);
    }

    [SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types")]
    public interface IMapper<in TSource1, in TSource2, out TDestination>
    {
        TDestination Map(TSource1 firstSource, TSource2 secondSource);
    }
}