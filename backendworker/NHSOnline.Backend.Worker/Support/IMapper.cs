namespace NHSOnline.Backend.Worker.Support
{
    public interface IMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource source);
    }

    public interface IMapper<in TSource1, in TSource2, out TDestination>
    {
        TDestination Map(TSource1 firstSource, TSource2 secondSource);
    }
}