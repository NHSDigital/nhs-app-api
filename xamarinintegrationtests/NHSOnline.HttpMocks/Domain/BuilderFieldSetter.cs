using System;

namespace NHSOnline.HttpMocks.Domain
{
    internal sealed class BuilderFieldSetter<TBuilder>
    {
        private readonly TBuilder _builder;

        internal BuilderFieldSetter(TBuilder builder) => _builder = builder;

        internal TBuilder Set(Action<TBuilder> setter)
        {
            setter(_builder);
            return _builder;
        }
    }
}