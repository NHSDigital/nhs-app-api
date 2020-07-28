using Moq;

namespace NHSOnline.Backend.PfsApi.UnitTests
{
    public class ArgumentCaptor<T>
    {
        public T Value { get; private set; }

        public T Capture()
        {
            return It.Is<T>(v => CaptureValue(v));
        }

        private bool CaptureValue(T value)
        {
            Value = value;

            return true;
        }
    }
}
