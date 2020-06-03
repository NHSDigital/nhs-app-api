using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSOnline.Backend.Auth.AspNet
{
    public sealed class UserProfileBinder: IModelBinder
    {
        /// <summary>
        /// This prevents the default model binder from attempting to create a user profile instance.
        ///
        /// The parameter will be populated by the <see cref="UserProfileFilter"/>.
        /// </summary>
        public Task BindModelAsync(ModelBindingContext bindingContext) => Task.CompletedTask;
    }
}