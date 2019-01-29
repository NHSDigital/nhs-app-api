using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers
{
    public static class EmbeddedResourceFileHelper
    {
        public static async Task<string> ReadEmbeddedResource(string resourceFile)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(resourceFile);

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}