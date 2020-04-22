using System;
using System.Net.Http;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal interface ITppClientRequestBuilder
    {
        string RequestType { get; }
        Guid Uuid { get; }

        ITppClientRequestBuilder Model<TRequest>(TRequest model) where TRequest : ITppRequest;
        ITppClientRequestBuilder Suid(string suid);
        HttpRequestMessage Build();
    }
}
