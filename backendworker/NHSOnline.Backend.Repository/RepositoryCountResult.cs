namespace NHSOnline.Backend.Repository
{
    public abstract class RepositoryCountResult
    {
        public abstract TOut Accept<TOut>(IRepositoryCountResultVisitor<TOut> visitor);

        public class RepositoryError : RepositoryCountResult
        {
            public override TOut Accept<TOut>(IRepositoryCountResultVisitor<TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Found : RepositoryCountResult
        {
            public long Count { get; }

            public Found(long count)
            {
                Count = count;
            }

            public override TOut Accept<TOut>(IRepositoryCountResultVisitor<TOut> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}