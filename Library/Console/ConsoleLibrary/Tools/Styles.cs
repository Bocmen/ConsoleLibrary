using System.Runtime.CompilerServices;

namespace ConsoleLibrary.Tools
{
    public static class Styles
    {
        internal static string CreateNameStyleConsoleLibrary([CallerFilePath] string callerFilePath = "", [CallerMemberName] string name = "") => NameStyleGenerator(nameof(ConsoleLibrary), callerFilePath, name);
        public static string CreateNameStyle(string nameLabrary, [CallerFilePath] string pathFile = "", [CallerMemberName] string propertyName = "") => NameStyleGenerator(nameLabrary, pathFile, propertyName);

        public static string NameStyleGenerator(string nameLabrary, string pathFile, string propertyName)
        {
            int indexSeparator = pathFile.LastIndexOf('/'); if (indexSeparator == -1) indexSeparator = pathFile.LastIndexOf('\\');
            if (indexSeparator != -1) pathFile = pathFile.Substring(indexSeparator + 1, pathFile.Length - indexSeparator - 1);
            indexSeparator = pathFile.LastIndexOf('.');
            if (indexSeparator != -1) pathFile = pathFile.Substring(0, indexSeparator);
            return $"{nameLabrary}_{pathFile.Replace(".xaml", string.Empty)}_{propertyName}";
        }
    }
}
