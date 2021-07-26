using System.Collections.Concurrent;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public class FixedSizeDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        private readonly int _maxSize;
        private readonly ConcurrentQueue<TKey> _keys;

        public FixedSizeDictionary(int size)
        {
            _maxSize = size;
            _keys = new ConcurrentQueue<TKey>();
        }

        public new void TryAdd(TKey key, TValue value)
        {
            base.TryAdd(key, value);
            _keys.Enqueue(key);

            if (_keys.Count > _maxSize)
            {
                _keys.TryDequeue(out var dequeueKey);
                base.TryRemove(dequeueKey, out _);
            }
        }
    }
}