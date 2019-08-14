using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure.Steps
{
    internal class OperationStepBuilder : IOperationStepBuilder
    {
        private readonly Stack<IOperationStep> _steps;
        private readonly IServiceProvider _services;

        public OperationStepBuilder(IServiceProvider services)
        {
            _steps = new Stack<IOperationStep>();
            _services = services;
        }

        public IOperationStepBuilder Add<TOperationStep>() where TOperationStep : IOperationStep
        {
            var step = _services.GetService<TOperationStep>();
            _steps.Push(step);
            
            return this;
        }

        public Task<RegistrationResult> Execute(RegistrationDescription registration, NotificationRegistrationRequest request)
        {
            var current = _steps.Pop();

            while (_steps.Any())
            {
                var previous = _steps.Pop();
                previous.NextStep(current);

                current = previous;
            }

            return current.Execute(registration, request);
        }
    }
}