using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Paycasso
{
    public interface IPaycassoResultVisitor
    {
        Task Visit(PaycassoResult.Success success);
        Task Visit(PaycassoResult.Failure failure);
    }
}