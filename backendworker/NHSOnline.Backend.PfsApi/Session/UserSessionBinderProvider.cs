using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            if (IsUserSessionParameter(context.Metadata))
            {
                return new BinderTypeModelBinder(typeof(UserSessionBinder));
            }

            return null;
        }

        private static bool IsUserSessionParameter(ModelMetadata modelMetadata)
        {
            return modelMetadata.MetadataKind == ModelMetadataKind.Parameter &&
                   typeof(UserSession).IsAssignableFrom(modelMetadata.ModelType);
        }
    }
}