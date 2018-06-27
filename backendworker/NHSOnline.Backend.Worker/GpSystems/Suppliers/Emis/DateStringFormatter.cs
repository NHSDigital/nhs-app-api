using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class DateStringFormatter 
    {
        public string Format(DateTimeOffset dateTime)
        {
            return dateTime.ToString("s");
        }
    }
}
