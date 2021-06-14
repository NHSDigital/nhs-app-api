using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;

namespace NHSOnline.App.Services.ForcedUpdate
{
    internal class AssessUpdateRequiredVisitor : IGetConfigurationResultVisitor<UpdateRequired>
    {
        private readonly INativeMinimumVersionCheck _nativeMinimumVersionCheck;

        public AssessUpdateRequiredVisitor(INativeMinimumVersionCheck nativeMinimumVersionCheck)
        {
            _nativeMinimumVersionCheck = nativeMinimumVersionCheck;
        }

        public UpdateRequired Visit(GetConfigurationResult.Success success)
        {
            return _nativeMinimumVersionCheck.MeetsMinimumVersion(success.VersionConfiguration)
                ? UpdateRequired.No
                : UpdateRequired.Yes;
        }

        public UpdateRequired Visit(GetConfigurationResult.Failed failed)
        {
            return UpdateRequired.Failed;
        }

        public UpdateRequired Visit(GetConfigurationResult.BadRequest badRequest)
        {
            return UpdateRequired.Failed;
        }
    }
}