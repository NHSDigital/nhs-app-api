using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure.Steps
{
    public interface IOperationStep
    {
        void NextStep(IOperationStep nextStep);
        Task<RegistrationResult> Execute(RegistrationDescription registration, string devicePns);
    }
}