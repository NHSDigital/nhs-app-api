using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public interface IOdsCodeMassager
    {
        bool IsEnabled { get; }

        string CheckOdsCode(string odsCode);
    }
}
