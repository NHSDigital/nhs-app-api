using System;

namespace NHSOnline.Backend.Worker.Support.TermsAndConditions
{
    public interface ITermsAndConditionsConfig
    {
        Uri EndpointUri { get; }
        string AuthKey { get; }
        string DatabaseId { get; }
        string CollectionName  { get; }
        bool Stubbed { get; }
    }
}