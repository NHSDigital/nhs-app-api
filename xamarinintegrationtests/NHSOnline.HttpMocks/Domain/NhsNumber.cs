using System;
using System.Globalization;
using Hl7.Fhir.Utility;

namespace NHSOnline.HttpMocks.Domain
{
    public abstract class NhsNumber
    {
        private NhsNumber()
        {}

        public static NhsNumber None { get; } = new NoneNhsNumber();
        public static NhsNumber FromInt(int intvalue) => new IntNhsNumber(intvalue);
        public static NhsNumber FromString(string stringValue) => new StringNHSNumber(stringValue);
        public abstract int IntValue { get; }
        public abstract string StringValue { get; }
        public abstract string FormattedStringValue { get; }

        private sealed class StringNHSNumber : NhsNumber
        {
            public StringNHSNumber(string stringValue)
            {
                IntValue = int.Parse(stringValue, CultureInfo.InvariantCulture);
                StringValue = stringValue.FormatWith(CultureInfo.InvariantCulture);
                FormattedStringValue = stringValue.FormatWith("000 000 0000", CultureInfo.InvariantCulture);
            }

            public override int IntValue { get; }
            public override string StringValue { get; }
            public override string FormattedStringValue { get; }
        }

        private sealed class IntNhsNumber : NhsNumber
        {
            public IntNhsNumber(int intValue)
            {
                IntValue = intValue;
                StringValue = intValue.ToString(CultureInfo.InvariantCulture);
                FormattedStringValue = intValue.ToString("000 000 0000", CultureInfo.InvariantCulture);
            }

            public override int IntValue { get; }
            public override string StringValue { get; }
            public override string FormattedStringValue { get; }
        }

        private sealed class NoneNhsNumber : NhsNumber
        {
            public override int IntValue => 0;
            public override string StringValue => string.Empty;
            public override string FormattedStringValue => string.Empty;
        }
    }
}