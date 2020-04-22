using System;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public abstract class CreateUserSessionResult: IAuditedResult
    {
        internal static CreateUserSessionResult Succeeded(UserSession userSession)=> new Success(userSession);
        internal static CreateUserSessionResult Failed(ErrorTypes errorType, string details) => new Failure(errorType, details);

        public abstract string Details { get; }

        internal abstract TResult Accept<TResult>(Func<Failure, TResult> onFailure, Func<Success, TResult> onSuccess);

        internal sealed class Success: CreateUserSessionResult
        {
            internal Success(UserSession result) => UserSession = result;

            internal UserSession UserSession { get; }
            public override string Details => "Session successfully created.";

            internal override TResult Accept<TResult>(Func<Failure, TResult> onFailure, Func<Success, TResult> onSuccess) => onSuccess(this);
        }

        internal sealed class Failure: CreateUserSessionResult
        {
            internal Failure(ErrorTypes errorType, string details)
            {
                ErrorType = errorType;
                Details = details;
            }

            internal ErrorTypes ErrorType { get; }
            public override string Details { get; }

            internal override TResult Accept<TResult>(Func<Failure, TResult> onFailure, Func<Success, TResult> onSuccess) => onFailure(this);
        }
    }
}
