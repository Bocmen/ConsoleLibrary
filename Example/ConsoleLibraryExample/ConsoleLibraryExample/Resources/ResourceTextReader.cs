using System.IO;
using System.Reflection;

namespace ConsoleLibraryExample.Resources
{
    public static class ResourceTextReader
    {
        private static readonly Assembly assembly = Assembly.GetAssembly(typeof(ResourceTextReader));
        public static StreamReader GetReader(string fileName) => new StreamReader(assembly.GetManifestResourceStream($"{nameof(ConsoleLibraryExample)}.{nameof(Resources)}.{fileName}"));
    }
}
