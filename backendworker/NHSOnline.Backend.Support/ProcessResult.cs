using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.Support
{
    [SuppressMessage(
        "Design",
        "CA1000:Do not declare static members on generic types",
        Justification = "Implicit operators apply to generic type")]
    public abstract class ProcessResult<TResult, TFailure>
    {
        private ProcessResult()
        {
        }

        public abstract bool Failed(out TFailure failure);
        public abstract TResult Result { get; }

        public static implicit operator TResult(ProcessResult<TResult, TFailure> processResult) => processResult.ToTResult();
        public TResult ToTResult() => Result;

        public static implicit operator ProcessResult<TResult, TFailure>(TResult result) => FromTResult(result);
        public static ProcessResult<TResult, TFailure> FromTResult(TResult result) => new SuccessResult(result);

        public static implicit operator ProcessResult<TResult, TFailure>(TFailure failure) => FromTFailure(failure);
        public static ProcessResult<TResult, TFailure> FromTFailure(TFailure failure) => new FailureResult(failure);

        private sealed class SuccessResult : ProcessResult<TResult, TFailure>
        {
            private readonly TResult _result;
            private Func<TResult> _getResult = CheckFailed;
            
            public SuccessResult(TResult value) => _result = value;

            public override bool Failed(out TFailure failure)
            {
                _getResult = () => _result;
                failure = default;
                return false;
            }

            public override TResult Result => _getResult();

            private static TResult CheckFailed()
                => throw new InvalidOperationException($"{nameof(Failed)} must be called before accessing {nameof(Result)}");
        }

        private sealed class FailureResult : ProcessResult<TResult, TFailure>
        {
            private readonly TFailure _value;

            public FailureResult(TFailure value) => _value = value;

            public override bool Failed(out TFailure failure)
            {
                failure = _value;
                return true;
            }

            public override TResult Result => throw new InvalidOperationException("The process has ended early");
        }
    }
}