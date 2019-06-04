using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public class VisionLinkagePostErrorMapper : VisionLinkageErrorMapper<LinkageKeyPostResponse>
    {
        protected override Im1ConnectionErrorCodes.Code UnknownError => Im1ConnectionErrorCodes.Code.UnknownError;

        protected override KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.Code>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.Code>()
                .AddKeyToEnum("409V2214",
                    Im1ConnectionErrorCodes.Code.LinkageKeyAlreadyExists);
    }
}
