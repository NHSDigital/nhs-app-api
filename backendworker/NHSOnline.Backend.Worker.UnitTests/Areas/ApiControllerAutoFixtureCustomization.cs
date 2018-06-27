using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace NHSOnline.Backend.Worker.UnitTests.Areas
{
    /// <summary>
    /// This is an AutoFixture customization to allow AutoFixture with MOQ Auto-mocking to create instances of ASP.NET Core Controllers.
    /// </summary>
    public class ApiControllerAutoFixtureCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<ICompositeMetadataDetailsProvider>(() => new CustomCompositeMetadataDetailsProvider());
            fixture.Inject(new ViewDataDictionary(fixture.Create<DefaultModelMetadataProvider>(), fixture.Create<ModelStateDictionary>()));
        }

        private class CustomCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
        {
            public void CreateBindingMetadata(BindingMetadataProviderContext context)
            {
            }

            public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
            {
            }

            public void CreateValidationMetadata(ValidationMetadataProviderContext context)
            {
            }
        }
    }
}
