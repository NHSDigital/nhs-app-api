using System.Globalization;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class NhsNumber
    {
        public NhsNumber(int intValue)
        {
            IntValue = intValue;
            StringValue = intValue.ToString(CultureInfo.InvariantCulture);
            FormattedStringValue = intValue.ToString("000 000 0000", CultureInfo.InvariantCulture);
        }

        public int IntValue { get; }
        public string StringValue { get; }
        public string FormattedStringValue { get; }
    }
}