using System;

namespace NHSOnline.App.Api.Client
{
    internal sealed class ModelValidationResultVisitor<TResponse, TResult>
        : IModelValidationResultVisitor<TResponse, TResult>
    {
        private readonly Func<TResponse, TResult> _onValid;
        private readonly Func<TResult> _onInvalid;

        public ModelValidationResultVisitor(Func<TResponse, TResult> onValid, Func<TResult> onInvalid)
        {
            _onValid = onValid;
            _onInvalid = onInvalid;
        }

        public TResult Visit(ModelValidationResult<TResponse>.Valid valid) => _onValid(valid.Response);

        public TResult Visit(ModelValidationResult<TResponse>.Invalid invalid) => _onInvalid();
    }
}