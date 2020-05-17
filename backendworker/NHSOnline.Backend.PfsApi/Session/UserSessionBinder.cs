using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NHSOnline.Backend.PfsApi.Filters;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionBinder: IModelBinder
    {
        /// <summary>
        /// This prevents the default model binder from attempting to create a user session instance.
        ///
        /// The parameter will be populated by the <see cref="UserSessionFilter"/>.
        /// </summary>
        public Task BindModelAsync(ModelBindingContext bindingContext) => Task.CompletedTask;
    }
}