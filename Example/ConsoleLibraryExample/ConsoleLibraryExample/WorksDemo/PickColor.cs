using System.Threading.Tasks;
using System.Threading;

using Xamarin.Forms;

using WorkInvoker.Abstract;
using WorkInvoker.Attributes;
using ConsoleLibrary.ConsoleExtensions;

namespace ConsoleLibraryExample.WorksDemo
{
    [LoaderWorkBase("Пример выбора цвета", "")]
    public class PickColor : WorkBase
    {
        public override async Task Start(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
                await Console.WriteLine(ConsoleLibrary.Extensions.FormattedStringExtension.ColorPattern("Пример окрашивание текста в заданный цвет", await Console.ReadColor("Выберите цвет", defaultValue: Color.Red, token: token)));
        }
    }
}