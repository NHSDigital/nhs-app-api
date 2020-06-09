namespace NHSOnline.Backend.Repository
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

        public class RepositoryError : RepositoryDeleteResult<TRecord>
        {
            public override TOut Accept<TOut>(IRepositoryDeleteResultVisitor<TRecord, TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
