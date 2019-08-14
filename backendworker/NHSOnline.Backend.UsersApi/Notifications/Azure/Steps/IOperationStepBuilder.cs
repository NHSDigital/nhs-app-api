using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure.Steps
{
    internal interface IOperationStepBuilder
    {
        IOperationStepBuilder Add<TOperationStep>() where TOperationStep : IOperationStep;

        Task<RegistrationResult> Execute(RegistrationDescription registration,
            NotificationRegistrationRequest request);
    }
}