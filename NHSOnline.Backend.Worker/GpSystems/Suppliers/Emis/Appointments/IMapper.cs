using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public interface IMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource source);
        IEnumerable<TDestination> Map(IEnumerable<TSource> source);
    }
}