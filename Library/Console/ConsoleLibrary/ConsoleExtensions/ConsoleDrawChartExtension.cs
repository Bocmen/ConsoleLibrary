using ConsoleLibrary.Interfaces;
using OxyPlot.Xamarin.Forms;
using OxyPlot;
using System.Threading.Tasks;
using ConsoleLibrary.Extensions;

using Xamarin.Forms;

namespace ConsoleLibrary.ConsoleExtensions
{
    public static class ConsoleDrawChartExtension
    {
        public static Task DrawChartOxyPlot(this IConsole console, PlotModel model) => console.AddUIElement(CreateChartOxyPlot(model));

        public static View CreateChartOxyPlot(PlotModel model) => ConsoleDecorationExtension.CreateViewOnFullScreenMode((type) =>
        {
            model.AttachToView(null);
            return new PlotView() { Model = model };
        });
    }
}
