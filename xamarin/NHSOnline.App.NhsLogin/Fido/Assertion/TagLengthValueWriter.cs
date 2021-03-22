using System;
using System.IO;
using System.Threading.Tasks;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido.Assertion
{
    internal sealed class TagLengthValueWriter: ITagLengthValueWriter, IAsyncDisposable
    {
        private readonly MemoryStream _memoryStream = new MemoryStream(4096);

        public TagLengthValueValueWriter StartTag(ushort tag) => new TagLengthValueValueWriter(_memoryStream, tag);

        internal byte[] ToArray() => _memoryStream.ToArray();

        public async ValueTask DisposeAsync() => await _memoryStream.DisposeAsync().ResumeOnThreadPool();
    }
}