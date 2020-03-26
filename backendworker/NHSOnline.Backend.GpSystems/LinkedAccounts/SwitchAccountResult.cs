namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class SwitchAccountResult
    {
        public abstract T Accept<T>(ISwitchAccountResultVisitor<T> visitor);

        public class Success : SwitchAccountResult
        {
            public override T Accept<T>(ISwitchAccountResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Failure : SwitchAccountResult
        {
            public override T Accept<T>(ISwitchAccountResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}