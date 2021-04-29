using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;

namespace NHSOnline.Backend.PfsApi.Resources
{
    public static class EmbeddedResources
    {
        public const string IntroductoryMessage = "Resources.Markdown.IntroductoryMessage.md";

        private static readonly ConcurrentDictionary<string, string> Resources = new ConcurrentDictionary<string, string>();

        public static string GetEmbeddedResource(string resource)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentOutOfRangeException(nameof(resource), resource);
            }

            if (!Resources.ContainsKey(resource))
            {
                Resources.TryAdd(resource, Retrieve(resource));
            }

            return Resources[resource];
        }

        private static string Retrieve(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            var resourceStream = assembly.GetManifestResourceStream($"{assemblyName}.{resource}");

            if (resourceStream == null)
            {
                throw new ArgumentOutOfRangeException(nameof(resource), resource);
            }

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);

            return reader.ReadToEndAsync().Result;
        }
    }
}
