using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public interface ITppClient
    {
        Task<AuthenticateReply> AuthenticatePost();
    }
}