using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Router;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class DateStringFormatter 
    {
        public string Format(DateTimeOffset dateTime)
        {
            return dateTime.ToString("s");
        }
    }
}
