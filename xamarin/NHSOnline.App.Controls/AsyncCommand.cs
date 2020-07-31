using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NHSOnline.App.Controls
{
    public sealed class AsyncCommand : ICommand
    {
        private readonly Func<Func<Task>?> _handlerAccessor;

        public AsyncCommand(Func<Func<Task>?> handlerAccessor)
        {
            _handlerAccessor = handlerAccessor;
        }

        public bool CanExecute(object parameter) => parameter is null;

        public void Execute(object? parameter)
        {
            var handler = _handlerAccessor();
            if (handler != null)
            {
                NhsAppResilience.ExecuteOnMainThread(() => handler());
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }
    }

    public sealed class AsyncCommand<T> : ICommand
    {
        private readonly Func<Func<T, Task>?> _handlerAccessor;

        public AsyncCommand(Func<Func<T, Task>?> handlerAccessor)
        {
            _handlerAccessor = handlerAccessor;
        }

        public bool CanExecute(object parameter) => parameter is T;

        public void Execute(object parameter)
        {
            if (parameter is T argument)
            {
                var handler = _handlerAccessor();
                if (handler != null)
                {
                    NhsAppResilience.ExecuteOnMainThread(() => handler(argument));
                }
            }
            else
            {
                throw new ArgumentException(
                    $"Parameter of type {typeof(T).FullName} expected but got {parameter?.GetType().FullName ?? "{null}"}",
                    nameof(parameter));
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}