using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FakeGpAreaBehaviourAttribute : Attribute
    {
        public Behaviour Behaviour { get; }

        public FakeGpAreaBehaviourAttribute(Behaviour behaviour)
        {
            Behaviour = behaviour;
        }
    }
}
