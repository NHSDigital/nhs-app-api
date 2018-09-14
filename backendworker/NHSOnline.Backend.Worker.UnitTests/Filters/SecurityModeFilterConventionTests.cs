using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Im1Connection;
using NHSOnline.Backend.Worker.Filters;

namespace NHSOnline.Backend.Worker.UnitTests.Filters
{
    [TestClass]
    public class SecurityModeFilterConventionTests
    {
        /// <summary>
        /// All endpoints in backend worker should be decorated with a subclass of SecurityModeAttribute, either
        /// at the class or method level.
        /// </summary>
        [TestMethod]
        public void FilterTest_AllEndpointsShouldHaveASecurityModeFilter()
        {    
            var endpointsWithoutSecurityModeFilter =
                Assembly.GetAssembly(typeof(Im1ConnectionController))
                    .GetTypes()
                    .Where(t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract)
                    .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => m.IsPublic && !m.IsDefined(typeof(NonActionAttribute)))
                    .Where(m => !m.GetCustomAttributes(typeof(SecurityModeAttribute), true).Any())
                    .Where(m => !m.DeclaringType.GetCustomAttributes(typeof(SecurityModeAttribute), true).Any())
                    .ToList();

            if (endpointsWithoutSecurityModeFilter.Any())
            {
                var endPointNames = endpointsWithoutSecurityModeFilter.Select(x => x.DeclaringType.Name + " " + x.Name);
                Assert.Fail("The following endpoints are missing a SecurityModeFilter:\r\n" + string.Join("\r\n",endPointNames));
            }
        }
    }
}