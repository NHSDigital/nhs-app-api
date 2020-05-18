using System;

namespace NHSOnline.Backend.Support.Repository
{
    public abstract class RepositoryDeleteResult<TRecord>
    {
        public abstract TOut Accept<TOut>(IRepositoryDeleteResultVisitor<TRecord, TOut> visitor);
        
        public class Deleted : RepositoryDeleteResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryDeleteResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : RepositoryDeleteResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryDeleteResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : RepositoryDeleteResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryDeleteResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class RepositoryError : RepositoryDeleteResult<TRecord>
        {
            public Exception Exception { get; }

            public RepositoryError(Exception exception)
            {
                Exception = exception;
            }

            public override TOut Accept<TOut>(IRepositoryDeleteResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
