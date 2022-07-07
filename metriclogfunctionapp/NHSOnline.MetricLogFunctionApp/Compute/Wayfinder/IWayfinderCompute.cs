using System;
using System.Threading.Tasks;

namespace NHSOnline.MetricLogFunctionApp.Compute.Wayfinder
{
    public interface IWayfinderCompute
    {
        public Task Execute(DateTime startDateTime, DateTime endDateTime);
    }
}
