using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public abstract class BaseMapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public abstract TDestination Map(TSource source);

        public IEnumerable<TDestination> Map(IEnumerable<TSource> source)
        {
            if (source == null || !source.Any())
            {
                return Enumerable.Empty<TDestination>();
            }

            return source.Select(Map);
        }
    }
}