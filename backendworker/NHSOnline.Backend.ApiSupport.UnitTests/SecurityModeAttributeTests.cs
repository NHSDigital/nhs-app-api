using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;

namespace NHSOnline.Backend.ApiSupport.UnitTests
{
    [TestClass]
    public class SecurityModeAttributeTests
    {
        /// <summary>
        /// All endpoints in the apis' should be decorated with a subclass of SecurityModeAttribute, either
        /// at the class or method level.
        /// </summary>
        [TestMethod]
        public void ConventionTest_AllEndpointsShouldHaveASecurityModeAttribute()
        {    
            var endpointsWithoutSecurityModeAttribute =
                Assembly.GetAssembly(typeof(Im1ConnectionController))
                    .GetTypes()
                    .Where(t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract)
                    .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => m.IsPublic && !m.IsDefined(typeof(NonActionAttribute)))
                    .Where(m => !m.GetCustomAttributes(typeof(SecurityModeAttribute), true).Any())
                    .Where(m => !m.DeclaringType.GetCustomAttributes(typeof(SecurityModeAttribute), true).Any())
                    .ToList();

            if (endpointsWithoutSecurityModeAttribute.Any())
            {
                var endPointNames = endpointsWithoutSecurityModeAttribute.Select(x => x.DeclaringType.Name + " " + x.Name);
                Assert.Fail("The following endpoints are missing a SecurityModeAttribute:\r\n" + string.Join("\r\n",endPointNames));
            }
        }
    }
}