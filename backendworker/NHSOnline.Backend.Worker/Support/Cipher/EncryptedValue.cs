using System;

namespace NHSOnline.Backend.Worker.Support.Cipher
{
    public class EncryptedValue
    {
        private const char SerialisedPartSeparator = ';';

        public byte[] InitialisationVector { get; }
        public byte[] EncryptedData { get; }

        public EncryptedValue(byte[] initialisationVector, byte[] encryptedData)
        {
            InitialisationVector = initialisationVector;
            EncryptedData = encryptedData;
        }

        public static EncryptedValue Parse(string serialisedEncryptedValue)
        {
            var serialisedParts = serialisedEncryptedValue?.Split(SerialisedPartSeparator);

            if ((serialisedParts?.Length ?? 0) != 2)
            {
                throw new ArgumentException(
                    $"${nameof(serialisedEncryptedValue)} must be a value that originated from a serialised {nameof(EncryptedValue)}");
            }

            var initialisationVector = Convert.FromBase64String(serialisedParts[0]);
            var encryptedData = Convert.FromBase64String(serialisedParts[1]);

            return new EncryptedValue(initialisationVector, encryptedData);
        }

        public override string ToString()
        {
            return string.Join(
                SerialisedPartSeparator,
                Convert.ToBase64String(InitialisationVector),
                Convert.ToBase64String(EncryptedData));
        }
    }
}