using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Support.Repository
{
    public abstract class RepositoryFindResult<TRecord>
    {
        public abstract TOut Accept<TOut>(IRepositoryFindResultVisitor<TRecord, TOut> visitor);

        public class NotFound : RepositoryFindResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryFindResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : RepositoryFindResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryFindResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class RepositoryError : RepositoryFindResult<TRecord>
        {
            public Exception Exception { get; }

            public RepositoryError(Exception exception)
            {
                Exception = exception;
            }

            public override TOut Accept<TOut>(IRepositoryFindResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Found : RepositoryFindResult<TRecord>
        {
            public IEnumerable<TRecord> Records { get; }

            public Found(IEnumerable<TRecord> records)
            {
                Records = records;
            }

            public override TOut Accept<TOut>(IRepositoryFindResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
