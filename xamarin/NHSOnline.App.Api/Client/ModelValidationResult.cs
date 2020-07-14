using System;

namespace NHSOnline.App.Api.Client
{
    internal abstract class ModelValidationResult<TResponse>
    {
        private ModelValidationResult()
        {}

        internal abstract TResult Accept<TResult>(IModelValidationResultVisitor<TResponse, TResult> visitor);

        internal TResult Accept<TResult>(
            Func<TResponse, TResult> onValid,
            Func<TResult> onInvalid)
        {
            var visitor = new ModelValidationResultVisitor<TResponse, TResult>(onValid, onInvalid);
            return Accept(visitor);
        }

        internal sealed class Valid: ModelValidationResult<TResponse>
        {
            public Valid(TResponse response) => Response = response;

            internal TResponse Response { get; }

            internal override TResult Accept<TResult>(IModelValidationResultVisitor<TResponse, TResult> visitor)
                => visitor.Visit(this);
        }

        internal sealed class Invalid : ModelValidationResult<TResponse>
        {
            public Invalid()
            { }

            internal override TResult Accept<TResult>(IModelValidationResultVisitor<TResponse, TResult> visitor)
                => visitor.Visit(this);
        }
    }
}