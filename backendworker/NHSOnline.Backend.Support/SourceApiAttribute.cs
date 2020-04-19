using System;

namespace NHSOnline.Backend.Support
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class SourceApiAttribute : Attribute
    {
        public SourceApi SourceApi { get; }

        public SourceApiAttribute(SourceApi sourceApi)
        {
            SourceApi = sourceApi;
        }
    }
}