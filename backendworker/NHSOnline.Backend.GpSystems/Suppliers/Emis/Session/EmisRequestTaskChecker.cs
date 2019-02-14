using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Session
{
    public class EmisRequestTaskChecker<T>
    {
        private readonly ILogger _logger;
        private readonly string _taskName;

        public EmisRequestTaskChecker(ILogger logger, string taskName)
        {
            _logger = logger;
            _taskName = taskName;
        }
        
        public T Check(Task<T> task)
        {
            string methodName = nameof(Check);
            _logger.LogDebug($"Entered {methodName}");
            
            T result = default(T);
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Emis " + _taskName + " task completed unsuccessfully");
            }
            else
            {
                _logger.LogDebug("Emis " + _taskName + " task completed successfully");
                result = task.Result;
            }

            _logger.LogDebug($"Exited {methodName}");
            return result;
        }       
    }
}