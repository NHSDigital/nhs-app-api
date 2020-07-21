using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public sealed class NhsLoginGetUserProfileDelayBehaviour : INhsLoginGetUserProfileBehaviour, IDisposable
    {
        public ManualResetEventSlim Continue { get; } = new ManualResetEventSlim(false);
        public ManualResetEventSlim Completed { get; } = new ManualResetEventSlim(false);

        private INhsLoginGetUserProfileBehaviour ContinueWith { get; } = new NhsLoginGetUserProfileDefaultBehaviour();

        public IActionResult Behave(Patient patient)
        {
            try
            {
                Continue.Wait();

                return ContinueWith.Behave(patient);
            }
            finally
            {
                Completed.Set();
            }
        }

        public void Dispose()
        {
            try
            {
                Continue.Set();
                Completed.Wait(TimeSpan.FromSeconds(2));

                Continue.Dispose();
                Completed.Dispose();
            }
            catch
            {}
        }
    }
}