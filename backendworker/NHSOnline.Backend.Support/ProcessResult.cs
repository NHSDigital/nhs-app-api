using System;

namespace NHSOnline.Backend.Support
{
    public static class ProcessResult
    {
        public static ProcessResult<TStepResult, TFinalResult> StepResult<TStepResult, TFinalResult>(TStepResult stepResult)
            => new ProcessResult<TStepResult, TFinalResult>.StepResult(stepResult);

        public static ProcessResult<TStepResult, TFinalResult> FinalResult<TStepResult, TFinalResult>(TFinalResult finalResult)
            => new ProcessResult<TStepResult, TFinalResult>.FinalResult(finalResult);
    }

    public abstract class ProcessResult<TStepResult, TFinalResult>
    {
        private ProcessResult()
        {
        }

        public abstract bool ProcessFinishedEarly(out TFinalResult finalResult);
        public TFinalResult ErrorResult => throw new InvalidOperationException("No error present in this result");
        public abstract TStepResult Result { get; }

        internal sealed class StepResult : ProcessResult<TStepResult, TFinalResult>
        {
            public StepResult(TStepResult value) => Result = value;

            public override bool ProcessFinishedEarly(out TFinalResult finalResult)
            {
                finalResult = default;
                return false;
            }

            public override TStepResult Result { get; }
        }

        internal sealed class FinalResult : ProcessResult<TStepResult, TFinalResult>
        {
            private readonly TFinalResult _value;

            public FinalResult(TFinalResult value) => _value = value;

            public override bool ProcessFinishedEarly(out TFinalResult finalResult)
            {
                finalResult = _value;
                return true;
            }

            public new TFinalResult ErrorResult => _value;

            public override TStepResult Result => throw new InvalidOperationException("The process has ended early");
        }
    }
}