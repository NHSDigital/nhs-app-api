using System;

namespace NHSOnline.Backend.Support.Repository
{
    public abstract class RepositoryUpdateResult<TRecord>
    {
        public abstract TOut Accept<TOut>(IRepositoryUpdateResultVisitor<TRecord, TOut> visitor);

        public class Updated : RepositoryUpdateResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryUpdateResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : RepositoryUpdateResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryUpdateResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : RepositoryUpdateResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryUpdateResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class RepositoryError : RepositoryUpdateResult<TRecord>
        {
            public Exception Exception { get; }

            public RepositoryError(Exception exception)
            {
                Exception = exception;
            }

            public override TOut Accept<TOut>(IRepositoryUpdateResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
