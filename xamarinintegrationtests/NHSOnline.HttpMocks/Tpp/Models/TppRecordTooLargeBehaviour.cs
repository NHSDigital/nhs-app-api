using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Extensions;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class TppRecordTooLargeBehaviour : ITppRecordsBehaviour
    {
        private const string FileTooLargeErrorCode = "24";

        public IActionResult Behave(IHeaderDictionary responseHeaders, TppPatient patient)
        {
            responseHeaders.Add("Suid", patient.Id);

            return new ContentResult
            {
                Content = TooLargeRequestBinaryDataResult(),
                ContentType = "text/xml"
            };
        }

        private string TooLargeRequestBinaryDataResult()
        {
            var errorReply = new Error
            {
                Uuid = Guid.NewGuid(),
                ErrorCode = FileTooLargeErrorCode,
                UserFriendlyMessage = "File exceeds 2MB"
            };

            return XmlHelper.SerializeXml<Error>(errorReply);
        }
    }
}