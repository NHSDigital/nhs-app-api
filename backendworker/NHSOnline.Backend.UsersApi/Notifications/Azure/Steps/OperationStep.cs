using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure.Steps
{
    internal abstract class OperationStep : IOperationStep
    {
        protected readonly ILogger _logger;
        protected IOperationStep Next { get; private set; }

        protected OperationStep(ILogger logger)
        {
            _logger = logger;
        }
        
        public IOperationStep NextStep(IOperationStep nextStep)
        {
            Next = nextStep;
            return Next;
        }

        public abstract Task<RegistrationResult> Execute(RegistrationDescription registration,
            NotificationRegistrationRequest request);
    }
}