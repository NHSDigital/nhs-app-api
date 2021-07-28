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
                yield return new EmisPatient().WithProofLevel5().WithLogin("P5Patient").WithName(b => b.FamilyName("P5"));
                yield return new EmisPatient().WithProofLevel5().WithLogin("BadRequest").WithBehaviour(new NhsLoginAuthoriseBlankCodeBehaviour());
                yield return new EmisPatient().WithProofLevel5().WithLogin("BadResponse").WithBehaviour(new NhsLoginTokenBadGatewayBehaviour());
                yield return new EmisPatient().WithProofLevel5().WithLogin("Timeout").WithBehaviour(GetUserProfileDelayBehaviour);
                yield return new EmisPatient().WithProofLevel5().WithLogin("NoNhsNumber").WithNhsNumber(NhsNumber.None);
                yield return new EmisPatient().WithProofLevel5().WithLogin("TooYoung").WithAge(12, 300);
                yield return new EmisPatient(EmisPatientOds.NoOdsCode).WithLogin("UnknownSupplier");
                yield return new EmisPatient(EmisPatientOds.UnknownOdsCode).WithLogin("UnknownOdsCode");
                yield return new EmisPatient().WithLogin("EmisPatient").WithName(b => b.FamilyName("EMIS"));
                yield return new EmisPatient().WithLogin("EmisForbidden").WithBehaviour(new EmisCreateSessionForbiddenBehaviour());
                yield return new TppPatient().WithLogin("TppPatient").WithName(b => b.FamilyName("Tpp"));
                yield return new VisionPatient().WithLogin("VisionPatient").WithName(b => b.FamilyName("Vision"));
                yield return new EmisPatient(EmisPatientOds.AllSilversEnabled).WithLogin("EmisWithAllSilvers").WithName(b => b.FamilyName("AllSilvers"));
                yield return new EmisPatient(EmisPatientOds.Pkb).WithLogin("PKB").WithName(b => b.FamilyName("pkb"));
                yield return new EmisPatient(EmisPatientOds.Cie).WithLogin("CIE").WithName(b => b.FamilyName("cie"));
                yield return new EmisPatient(EmisPatientOds.SecondaryCareView).WithLogin("MCV").WithName(b => b.FamilyName("scv"));
                yield return new EmisPatient(EmisPatientOds.MyCareView).WithLogin("SCV").WithName(b => b.FamilyName("mcv"));
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
                    patient.Login,
                    name.Title,
                    name.GivenName,
                    name.FamilyName,
                    patient.GetType().Name);

                patients.Add(patient);
            }

            await using var _ = await MockWebServer.Start(patients, config => config.AddConsole());

            using var semaphore = new SemaphoreSlim(0, 1);
            Console.CancelKeyPress += (sender, eventArgs) => semaphore.Release();
            await semaphore.WaitAsync();
        }
    }
}
