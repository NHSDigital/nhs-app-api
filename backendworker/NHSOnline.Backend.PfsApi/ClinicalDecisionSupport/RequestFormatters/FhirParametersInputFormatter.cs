extern alias stu3;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using STU3Serialization = stu3::Hl7.Fhir.Serialization;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.RequestFormatters
{
    public class FhirParametersInputFormatter : TextInputFormatter
    {
        private readonly STU3Serialization.FhirJsonParser _parser;

        public FhirParametersInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(Constants.ContentTypes.ApplicationJsonFhir));
            SupportedEncodings.Add(Encoding.UTF8);

            _parser = new STU3Serialization.FhirJsonParser
            {
                Settings =
                {
                    AllowUnrecognizedEnums = true,
                    AcceptUnknownMembers = true
                }
            };
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            var request = context.HttpContext.Request.Body;

            using (var streamReader = new StreamReader(request))
            {
                Parameters parameters;

                try
                {
                    parameters = _parser.Parse<Parameters>(await streamReader.ReadToEndAsync().ConfigureAwait(false));
                }
                catch
                {
                    return await InputFormatterResult.FailureAsync();
                }

                return await InputFormatterResult.SuccessAsync(parameters);
            }
        }
    }
}