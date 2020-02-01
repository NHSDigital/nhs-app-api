using System;
using System.Threading;
using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestLock: IDisposable
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        internal async Task<IDisposable> Acquire()
        {
            await _lock.WaitAsync();

            return Disposable.Create(() => _lock.Release());
        }

        public void Dispose() => _lock.Dispose();
    }
}