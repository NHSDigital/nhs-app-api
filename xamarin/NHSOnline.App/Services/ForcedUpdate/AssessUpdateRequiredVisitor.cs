using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;

namespace NHSOnline.App.Services.ForcedUpdate
{
    internal class AssessUpdateRequiredVisitor : IGetConfigurationResultVisitor<UpdateRequired>
    {
        private readonly INativeAppVersionCheckService _nativeAppVersionCheckService;

        public AssessUpdateRequiredVisitor(INativeAppVersionCheckService nativeAppVersionCheckService)
        {
            _nativeAppVersionCheckService = nativeAppVersionCheckService;
        }

        public UpdateRequired Visit(GetConfigurationResult.Success success)
        {
            return _nativeAppVersionCheckService.MeetsMinimumVersion(success.VersionConfiguration)
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