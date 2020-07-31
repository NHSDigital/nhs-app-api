using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpUserSessionRecreateResultVisitor : GpUserSessionCreateResultVisitorBase<GpSessionRecreateResult>
    {
        private readonly ILogger _logger;
        private readonly Supplier _supplier;
        private readonly string _odsCode;

        public GpUserSessionRecreateResultVisitor(
            ILogger logger,
            Supplier supplier,
            string odsCode) : base(logger, supplier)
        {
            _logger = logger;
            _supplier = supplier;
            _odsCode = odsCode;
        }

        public override GpSessionRecreateResult Visit(GpSessionCreateResult.Success result) =>
            new GpSessionRecreateResult.RecreatedResult();

        protected override GpSessionRecreateResult Failed(ErrorTypes userSessionError, string message)
        {
            _logger.LogWarning(
                $"Recreating gp session for supplier={_supplier} odsCode={_odsCode} failed: {message}");

            return new GpSessionRecreateResult.ErrorResult(userSessionError, message);
        }
    }
}
