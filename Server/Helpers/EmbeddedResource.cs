using System;
using System.IO;
using System.Reflection;

namespace PowerSocketServer.Helpers
{
    public static class EmbeddedResource
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();

        public static bool HasResource(string resourceName)
        {
            return (Array.IndexOf(assembly.GetManifestResourceNames(), resourceName) > -1);
        }

        public static string GetResource(string resourceName)
        {
            string result;

            if (HasResource(resourceName)) {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
