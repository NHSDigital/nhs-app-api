using System.Diagnostics.CodeAnalysis;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Vision.Models;

namespace NHSOnline.HttpMocks.Vision
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Created by DI")]
    internal sealed class VisionConfigurationSoapRequestHandler : IVisionSoapRequestHandler
    {
        public string Method => "VOS.GetConfiguration";

        public VisionResponseEnvelope Handle(VisionPatient patient)
        {
            var name = patient.PersonalDetails.Name;
            var serviceContent = new PatientConfigurationResponse
            {
                Configuration = new PatientConfiguration
                {
                    Account = new Account
                    {
                        Name = $"{name.Title} {name.GivenName} {name.FamilyName}",
                        PatientId = patient.Id,
                        PatientNumbers =
                        {
                            new PatientNumber
                            {
                                Number = patient.NhsNumber.FormattedStringValue,
                                NumberType = "NHS"
                            }
                        }
                    },
                    Appointments = new AppointmentsConfiguration
                    {
                        BookingEnabled = true,
                        VisionMessage =
                        {
                            new VisionMessage
                            {
                                Text = "<HTML><HEAD><META name=GENERATOR content=\"MSHTML 11.00.9600.17207\"></HEAD><BODY><P>Book with our nurses for -- Immunisations \n\n Thank you</P></BODY></HTML>",
                                Language = "en_UK"

                            }
                        }
                    },
                    Prescriptions = new PrescriptionsConfiguration
                    {
                        RepeatEnabled = true
                    }
                }
            };

            return VisionResponseEnvelope.Create(Method, "2.3.0", serviceContent);
        }
    }
}