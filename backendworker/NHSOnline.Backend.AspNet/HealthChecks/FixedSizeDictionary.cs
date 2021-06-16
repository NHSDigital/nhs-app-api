using System.Collections.Generic;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public class FixedSizeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly int _maxSize;
        private readonly Queue<TKey> _keys;

        public FixedSizeDictionary(int size)
        {
            _maxSize = size;
            _keys = new Queue<TKey>();
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            _keys.Enqueue(key);

            if (_keys.Count > _maxSize)
            {
                base.Remove(_keys.Dequeue());
            }
        }
    }
}