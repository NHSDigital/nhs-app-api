using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public abstract class GpUserSessionCreateResultVisitorBase<T> : IGpSessionCreateResultVisitor<T>
    {
        private readonly ILogger _logger;
        private readonly Supplier _supplier;

        protected GpUserSessionCreateResultVisitorBase(ILogger logger, Supplier supplier)
        {
            _logger = logger;
            _supplier = supplier;
        }

        public abstract T Visit(GpSessionCreateResult.Success result);

        public T Visit(GpSessionCreateResult.Forbidden result) =>
            Failed(new ErrorTypes.LoginForbidden(), result.Message);

        public T Visit(GpSessionCreateResult.BadGateway result) =>
            Failed(ErrorTypes.LoginBadGateway(_logger, _supplier), result.Message);

        public T Visit(GpSessionCreateResult.InternalServerError result) =>
            Failed(new ErrorTypes.LoginUnexpectedError(), result.Message);

        public T Visit(GpSessionCreateResult.BadRequest result) =>
            Failed(new ErrorTypes.LoginBadRequest(), result.Message);

        public T Visit(GpSessionCreateResult.InvalidConnectionToken result) =>
            Failed(new ErrorTypes.LoginForbidden(), "Invalid connection token");

        public T Visit(GpSessionCreateResult.ErrorExceptionResult result) =>
            Failed(new ErrorTypes.LoginForbidden(), result.ErrorMessage);

        public T Visit(GpSessionCreateResult.Unparseable result) =>
            Failed(new ErrorTypes.LoginGPUnparseable(), result.ErrorMessage);

        public T Visit(GpSessionCreateResult.Timeout result) =>
            Failed(ErrorTypes.LoginTimeout(_logger, _supplier), result.ErrorMessage);

        protected abstract T Failed(ErrorTypes userSessionError, string message);
    }
}