using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;
using NHSOnline.HttpMocks.Microtest;
using NHSOnline.HttpMocks.Tpp;
using NHSOnline.HttpMocks.Vision;

namespace NHSOnline.HttpMocks.Dev
{
    class Program
    {
        private static IEnumerable<Patient> Patients
        {
            get
            {
                yield return new P5Patient().WithId("P5Patient").WithName(b => b.FamilyName("P5"));
                yield return new EmisPatient().WithId("EmisPatient").WithName(b => b.FamilyName("EMIS"));
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

            await using var _ = MockWebServer.Start(patients);

            using var semaphore = new SemaphoreSlim(0, 1);
            Console.CancelKeyPress += (sender, eventArgs) => semaphore.Release();
            await semaphore.WaitAsync();
        }
    }
}
