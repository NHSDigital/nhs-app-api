using System;

namespace NHSOnline.Backend.Support 
{
    public interface IGuidCreator
    {  
       Guid CreateGuid(); 
    }

    public class GuidCreator: IGuidCreator
    {
        public Guid CreateGuid()
        {
            return Guid.NewGuid();
        }
        
    }
}