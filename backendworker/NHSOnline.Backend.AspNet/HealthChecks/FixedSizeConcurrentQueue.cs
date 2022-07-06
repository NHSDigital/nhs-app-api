using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    // Disable 'Collections should implement generic interface' rule as we do not wish to expose all collection methods in this class
    [SuppressMessage("Microsoft.Design", "CA1010")]
    // Disable 'Identifiers should not have incorrect suffix' as the 'Queue' suffix expresses clearly the intent of this object.
    [SuppressMessage("Naming", "CA1711")]
    public class FixedSizeConcurrentQueue<T> : ConcurrentQueue<T>
    {
        private readonly int _maxSize;

        public FixedSizeConcurrentQueue(int maxSize)
        {
            _maxSize = maxSize;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);

            while (Count > _maxSize)
            {
                TryDequeue(out _);
            }
        }
    }
}