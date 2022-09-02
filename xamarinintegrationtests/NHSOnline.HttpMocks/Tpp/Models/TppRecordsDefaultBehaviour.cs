using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Extensions;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class TppRecordsDefaultBehaviour : ITppRecordsBehaviour
    {
        public IActionResult Behave(IHeaderDictionary responseHeaders, TppPatient patient)
        {
            responseHeaders.Add("Suid", patient.Id);

            return new ContentResult
            {
                Content = SuccessfulRequestBinaryDataResult(),
                ContentType = "text/xml"
            };
        }

        private string SuccessfulRequestBinaryDataResult()
        {
            var basePath =
                $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent}/Resources";

            var requestBinaryDataReply = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "txt",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = File.ReadAllText($"{basePath}/HandAndFootXrayImage.txt")
                    }
                }
            };

            return XmlHelper.SerializeXml<RequestBinaryDataReply>(requestBinaryDataReply);
        }
    }
}