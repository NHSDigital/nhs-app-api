using System;

namespace NHSOnline.MetricLogFunctionApp.Etl.Logging
{
    public interface IEtlLogger<TCategoryName>
    {
        void StartedTriggered(string tableName, string context);

        void Failed(string content = null);

        void Information(string content);
    }
}