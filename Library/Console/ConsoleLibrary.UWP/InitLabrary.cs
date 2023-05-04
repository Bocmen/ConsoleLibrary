using System.Collections.Generic;
using System.Reflection;

namespace ConsoleLibrary.Platform.UWP
{
    public static class InitLabrary
    {
        public static IEnumerable<Assembly> IncludeAssemblery;

        static InitLabrary()
        {
            List<Assembly> list = new List<Assembly>
            {
                typeof(OxyPlot.Xamarin.Forms.Platform.UWP.PlotViewRenderer).GetTypeInfo().Assembly
            };
            list.AddRange(Rg.Plugins.Popup.Popup.GetExtraAssemblies());
            IncludeAssemblery = list;
        }
        public static void Init()
        {
            OxyPlot.Xamarin.Forms.Platform.UWP.PlotViewRenderer.Init();
            Rg.Plugins.Popup.Popup.Init();
        }
    }
}
