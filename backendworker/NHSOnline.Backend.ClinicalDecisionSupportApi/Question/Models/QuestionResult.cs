namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Question.Models
{
    public abstract class QuestionResult
    {
        public abstract T Accept<T>(IQuestionResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : QuestionResult
        {
            public QuestionResponse Response { get; }

            public SuccessfullyRetrieved(QuestionResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IQuestionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : QuestionResult
        {
            public override T Accept<T>(IQuestionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : QuestionResult
        {
            public override T Accept<T>(IQuestionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}