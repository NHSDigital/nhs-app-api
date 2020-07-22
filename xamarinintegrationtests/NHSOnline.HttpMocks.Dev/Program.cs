using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;

namespace NHSOnline.HttpMocks.Dev
{
    class Program
    {
        private static readonly NhsLoginGetUserProfileDelayBehaviour GetUserProfileDelayBehaviour = new NhsLoginGetUserProfileDelayBehaviour();

        private static IEnumerable<Patient> Patients
        {
            get
            {
                yield return new P5Patient().WithId("P5Patient").WithName(b => b.FamilyName("P5"));
                yield return new P5Patient().WithId("BadRequest").WithBehaviour(new NhsLoginAuthoriseBlankCodeBehaviour());
                yield return new P5Patient().WithId("BadResponse").WithBehaviour(new NhsLoginTokenBadGatewayBehaviour());
                yield return new P5Patient().WithId("Timeout").WithBehaviour(GetUserProfileDelayBehaviour);
                yield return new P5Patient().WithId("NoNhsNumber").WithNhsNumber(NhsNumber.None);
                yield return new P5Patient().WithId("TooYoung").WithAge(12, 300);
                yield return new P9Patient().WithId("UnknownSupplier").WithUnknownSupplierOdsCode();
                yield return new P9Patient().WithId("UnknownOdsCode").WithUnknownOdsCode();
                yield return new EmisPatient().WithId("EmisPatient").WithName(b => b.FamilyName("EMIS"));
                yield return new EmisPatient().WithId("EmisForbidden").WithBehaviour(new EmisCreateSessionForbiddenBehaviour());
                yield return new TppPatient().WithId("TppPatient").WithName(b => b.FamilyName("Tpp"));
                yield return new MicrotestPatient().WithId("MicrotestPatient").WithName(b => b.FamilyName("Microtest"));
                yield return new VisionPatient().WithId("VisionPatient").WithName(b => b.FamilyName("Vision"));
            }
        }

        private static async Task Main()
        {
            var patients = new PatientsCollection();
            foreach (var patient in Patients)
            {
                var name = patient.PersonalDetails.Name;
                Console.WriteLine(
                    "{0}: {1} {2} {3} ({4})",
                    patient.Id,
                    name.Title,
                    name.GivenName,
                    name.FamilyName,
                    patient.GetType().Name);

                patients.Add(patient);
            }

            await using var _ = MockWebServer.Start(patients, config => config.AddConsole());

            using var semaphore = new SemaphoreSlim(0, 1);
            Console.CancelKeyPress += (sender, eventArgs) => semaphore.Release();
            await semaphore.WaitAsync();
        }
    }
}
