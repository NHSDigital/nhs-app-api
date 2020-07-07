using System;
using System.Collections.Generic;

namespace NHSOnline.HttpMocks.Domain
{
    internal sealed class Behaviours
    {
        private readonly Dictionary<Type, object> _behaviour = new Dictionary<Type, object>();

        internal void Add<TBehaviour>(TBehaviour behaviour) where TBehaviour : class
        {
            _behaviour.Add(typeof(TBehaviour), behaviour);
        }

        internal TBehaviour Get<TBehaviour>(Func<TBehaviour> defaultFunc) where TBehaviour : class
        {
            return _behaviour.GetValueOrDefault(typeof(TBehaviour)) as TBehaviour ?? defaultFunc();
        }
    }
}