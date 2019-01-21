namespace NHSOnline.Backend.Worker.Support
{
    internal interface IMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource source);
    }

    internal interface IMapper<in TSource1, in TSource2, out TDestination>
    {
        TDestination Map(TSource1 firstSource, TSource2 secondSource);
    }
}