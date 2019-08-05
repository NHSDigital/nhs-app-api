using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace NHSOnline.Backend.Support.UnitTests.Middleware
{
    public class DummyResponseFeature : IHttpResponseFeature
    {
        private Func<object, Task> _callback;
        private object _state;
        
        public Stream Body { get; set; }

        public bool HasStarted { get; private set; }

        public IHeaderDictionary Headers { get; set; }

        public string ReasonPhrase { get; set; }

        public int StatusCode { get; set; }

        public void OnCompleted(Func<object, Task> callback, object state)
        {
            //...No-op
        }

        public void OnStarting(Func<object, Task> callback, object state)
        {
            _callback = callback;
            _state = state;
        }
        
        public Task InvokeCallBack()
        {
            HasStarted = true;
            return _callback(_state);
        }
    }
}