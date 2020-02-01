using System;

namespace NHSOnline.Backend.Support
{
    public static class Disposable
    {
        public static IDisposable Create(Action dispose) => new ActionDisposable(dispose);

        private sealed class ActionDisposable: IDisposable
        {
            private readonly Action _dispose;
            private bool _disposed;

            public ActionDisposable(Action dispose) => _dispose = dispose;

            public void Dispose()
            {
                if (! _disposed)
                {
                    _dispose();
                    _disposed = true;
                }
            }
        }
    }
}