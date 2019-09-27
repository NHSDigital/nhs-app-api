using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace UnitTestHelper
{
    public static class ServiceCollectionHelper
    {
        public static IEnumerable<ServiceDescriptor> SetupServiceDescriptor(Mock<IServiceCollection> mockServiceCollection)
        {
            var serviceDescriptors = new List<ServiceDescriptor>();
            mockServiceCollection.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(x => serviceDescriptors.Add(x));
            return serviceDescriptors;
        }
    }
}