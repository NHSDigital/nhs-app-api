using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public interface ITppRecordsBehaviour
    {
        IActionResult Behave(IHeaderDictionary responseHeaders, TppPatient patient);
    }
}