using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Extensions;
using NHSOnline.HttpMocks.Tpp.Models;

namespace NHSOnline.HttpMocks.Tpp
{
    [Route("tpp")]
    public sealed class TppSessionController: TppBaseController
    {
        public TppSessionController(IPatients patients) : base(patients)
        {
        }

        [HttpPost]
        [TppTypeHeader("Authenticate")]
        public IActionResult Authenticate([FromBody] Authenticate request)
        {
            if (!ModelState.IsValid || request?.AccountId == null)
            {
                return BadRequest(ModelState);
            }

            var patientId = request.AccountId;
            var patient = GetPatients().LookupById(request.AccountId);
            if (patient is TppPatient tppPatient)
            {
                return ReturnXmlResult(patientId, BuildAuthenticateResult(tppPatient));
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a TPP patient");
        }

        [HttpPost]
        [TppTypeHeader("ListServiceAccesses")]
        public IActionResult ListServiceAccesses([FromBody] ListServiceAccesses request)
        {
            if (!ModelState.IsValid || request?.PatientId == null)
            {
                return BadRequest(ModelState);
            }

            var patientId = request.PatientId;
            var patient = GetPatients().LookupById(request.PatientId);
            if (patient is TppPatient tppPatient)
            {
                return ReturnXmlResult(patientId, BuildListServiceAccessesResult());
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a TPP patient");
        }

        private string BuildAuthenticateResult(TppPatient patient)
        {
            var person = TppDefaults.DefaultTppPerson(patient);

            var authenticateReply = new AuthenticateReply
            {
                PatientId = patient.Id,
                OnlineUserId = patient.Id,
                User = new User { Person = person },
                Uuid = new Guid(),
                Suid = patient.Id,
                People = { person },
                Registration = new Registration
                {
                    PatientAccess =
                    {
                        new PatientAccess
                        {
                            PatientId = patient.Id,
                            SiteDetails = new SiteDetails
                            {
                                UnitName = "Practice X",
                                Address = new Address
                                {
                                    AddressText = "1 Cool Drive, Leeds",
                                }
                            }
                        }
                    },
                }
            };

            return XmlHelper.SerializeXml<AuthenticateReply>(authenticateReply);
        }

        private string BuildListServiceAccessesResult()
        {
            var listServiceAccesses = new ListServiceAccessesReply
            {
                ServiceAccess =
                {
                    new ServiceAccess
                    {
                        Description = "Messaging",
                        ServiceIdentifier = "512",
                        Status = "A",
                        StatusDesc = "A"
                    }
                }
            };

            return XmlHelper.SerializeXml<ListServiceAccessesReply>(listServiceAccesses);
        }
    }
}