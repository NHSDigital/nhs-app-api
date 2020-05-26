using NHSOnline.Backend.Auditing;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch
{
    public abstract class PostUserResearchResult : IAuditedResult
    {
        public abstract T Accept<T>(IUserResearchResultVisitor<T> visitor);

        public class Success : PostUserResearchResult
        {
            public override string Details => "Successful post user research result";
            public override T Accept<T>(IUserResearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Failure : PostUserResearchResult
        {
            public override string Details => "Failure when attempting to post user research result";
            public override T Accept<T>(IUserResearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : PostUserResearchResult
        {
            public override string Details => "Internal Server Error when attempting to post user research result";
            public override T Accept<T>(IUserResearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class EmailMissing : PostUserResearchResult
        {
            public override string Details => "Email missing when attempting to post user research result";
            public override T Accept<T>(IUserResearchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public abstract string Details { get; }
    }
}