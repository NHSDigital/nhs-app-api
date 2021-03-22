using System;
using System.IO;
using System.Text;

namespace NHSOnline.App.NhsLogin.Fido.Assertion
{
    internal sealed class TagLengthValueValueWriter: ITagLengthValueWriter, IDisposable
    {
        private const ushort LengthPlaceHolder = 0;

        private readonly MemoryStream _memoryStream;
        private readonly long _lengthPosition;
        private readonly long _valueStartPosition;

        internal TagLengthValueValueWriter(MemoryStream memoryStream, ushort tag)
        {
            _memoryStream = memoryStream;
            Write(tag);
            _lengthPosition = _memoryStream.Position;
            Write(LengthPlaceHolder);
            _valueStartPosition = _memoryStream.Position;
        }

        public TagLengthValueValueWriter StartTag(ushort tag) => new TagLengthValueValueWriter(_memoryStream, tag);

        internal void Write(byte uint8) => _memoryStream.WriteByte(uint8);

        internal void Write(ushort uint16)
        {
            Write((byte)(uint16 & 0x00FF));
            Write((byte)((uint16 & 0xFF00) >> 8));
        }

        internal void Write(uint uint32)
        {
            Write((byte)(uint32 & 0x000000FF));
            Write((byte)((uint32 & 0x0000FF00) >> 8));
            Write((byte)((uint32 & 0x00FF0000) >> 16));
            Write((byte)((uint32 & 0xFF00000) >> 24));
        }

        internal void Write(byte[] bytes) => _memoryStream.Write(bytes);

        internal void Write(string text) => Write(Encoding.UTF8.GetBytes(text));

        public void Dispose()
        {
            var valueEndPosition = _memoryStream.Position;
            var length = (ushort)(valueEndPosition - _valueStartPosition);

            _memoryStream.Position = _lengthPosition;
            Write(length);
            _memoryStream.Position = valueEndPosition;
        }
    }
}