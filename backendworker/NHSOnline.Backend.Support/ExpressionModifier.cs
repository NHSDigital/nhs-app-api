using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace NHSOnline.Backend.Support
{
    // Class used to traverse and replace elements in an expression
    // See stackoverflow https://stackoverflow.com/a/71721612
    public class ExpressionModifier : ExpressionVisitor
    {
        private readonly Expression _expressionToReplace;
        private readonly Expression _expressionReplacement;

        private ExpressionModifier(Expression expressionToReplace, Expression expressionReplacement)
        {
            _expressionToReplace = expressionToReplace;
            _expressionReplacement = expressionReplacement;
        }

        [SuppressMessage("ReSharper", "CA1725", Justification = "Change name of parameter for clarity")]
        public override Expression Visit(Expression currentNode)
        {
            if (ReferenceEquals(currentNode, _expressionToReplace))
            {
                return _expressionReplacement;
            }
            return base.Visit(currentNode);
        }

        public static T Replace<T>(T target, Expression expressionToReplace, Expression expressionReplacement)
            where T : Expression
        {
            var replacer = new ExpressionModifier(expressionToReplace, expressionReplacement);
            return (T)replacer.Visit(target);
        }
    }
}