using System;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public abstract class CreateUserSessionResult
    {
        internal static CreateUserSessionResult Succeeded(UserSession userSession)=> new Success(userSession);
        internal static CreateUserSessionResult Failed(ErrorTypes errorType) => new Failure(errorType);

        internal abstract TResult Accept<TResult>(Func<Failure, TResult> onFailure, Func<Success, TResult> onSuccess);

        internal sealed class Success: CreateUserSessionResult
        {
            internal Success(UserSession result) => UserSession = result;

            internal UserSession UserSession { get; }

            internal override TResult Accept<TResult>(Func<Failure, TResult> onFailure, Func<Success, TResult> onSuccess) => onSuccess(this);
        }

        internal sealed class Failure: CreateUserSessionResult
        {
            internal Failure(ErrorTypes errorType) => ErrorType = errorType;

            internal ErrorTypes ErrorType { get; }

            internal override TResult Accept<TResult>(Func<Failure, TResult> onFailure, Func<Success, TResult> onSuccess) => onFailure(this);
        }
    }
}
