using ConsoleLibrary.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using ConsoleLibrary.Tools;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace ConsoleLibrary.ConsoleExtensions
{
    public static partial class ConsoleIOExtension
    {
        public static readonly string FilePickButtonStyle = Styles.CreateNameStyleConsoleLibrary();
        private const string ErrorMessageFileNotSelect = "Файл не выбран";
        private const string PatternMessageFileSelect = "Выбран файл: {0}";

        public static async Task<FileResult> GetFilePath(this IConsole console, string title, PickOptions pickOptions = null, OptionReadValue options = OptionReadValue.None, GenerateMessagesResultUseTitle<FileResult> getResultMassage = null, CancellationToken? token = null)
        {
            FileResult result = null;
            var btnPickFile = new Button()
            {
                Text = ErrorMessageFileNotSelect
            };
            btnPickFile.SetDynamicResource(VisualElement.StyleProperty, FilePickButtonStyle);
            btnPickFile.Clicked += async (d, dd) =>
            {
                var selectFile = await FilePicker.PickAsync(pickOptions);
                if (selectFile != null)
                {
                    result = selectFile;
                    await console.InvockeUITheardAction(() => btnPickFile.Text = string.Format(PatternMessageFileSelect, result.FullPath));
                }
            };
            var generatorMassegaResult = GetGeneratorResultMessage(getResultMassage, title);
            return await await console.ReadValue(title, btnPickFile, (v) =>
            {
                if (result != null) return ResultValue<FileResult>.CreateResult(result);
                return ResultValue<FileResult>.CreateError(ErrorMessageFileNotSelect);
            }, generatorMassegaResult, options, token);
        }
    }
}
