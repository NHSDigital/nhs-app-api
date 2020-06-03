using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.AspNet
{
    public class UserProfileBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            if (IsUserProfileParameter(context.Metadata))
            {
                return new BinderTypeModelBinder(typeof(UserProfileBinder));
            }

            return null;
        }

        private static bool IsUserProfileParameter(ModelMetadata modelMetadata)
        {
            return modelMetadata.MetadataKind == ModelMetadataKind.Parameter &&
                   typeof(UserProfile).IsAssignableFrom(modelMetadata.ModelType);
        }
    }
}