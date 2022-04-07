using System;

namespace NHSOnline.MetricLogFunctionApp.Compute.Logging
{
    public interface IComputeLogger<TCategoryName>
    {
        void Started(string tableName, DateTime startDateTime, DateTime endDateTime);

        public void DuplicatedDataFound();

        void Failed();
    }
}