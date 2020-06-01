using System;

namespace NHSOnline.Backend.Repository
{
    public abstract class RepositoryCreateResult<TRecord>
    {
        public abstract TOut Accept<TOut>(IRepositoryCreateResultVisitor<TRecord, TOut> visitor);

        public class Created : RepositoryCreateResult<TRecord>
        {

            public Created(TRecord record)
            {
                Record = record; 
            }

            public TRecord Record { get; set; }

            public override TOut Accept<TOut>(IRepositoryCreateResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : RepositoryCreateResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryCreateResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class RepositoryError : RepositoryCreateResult<TRecord>
        {
            public Exception Exception { get; }

            public RepositoryError(Exception exception)
            {
                Exception = exception;
            }

            public override TOut Accept<TOut>(IRepositoryCreateResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
