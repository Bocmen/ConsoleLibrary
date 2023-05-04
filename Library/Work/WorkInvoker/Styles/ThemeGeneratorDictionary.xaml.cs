using static WorkInvoker.Styles.Themes;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkInvoker.Styles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThemeGeneratorDictionary : ResourceDictionary
    {
        public static string TextColor = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static string BackgroundColor = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static string ForegroundColor = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static string UIElemColor = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static string PositiveColor = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static string NegativeColor = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static string CornerRadius = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static string TextSize_Text = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static string TextSize_Title = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static string LaTeXSize_Text = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static string LaTeXSize_Title = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static string OxyPlotChart_HeightScale = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public ThemeGeneratorDictionary(StyleGenerator styleGenerator)
        {
            Add(TextColor, styleGenerator.TextColor);
            Add(BackgroundColor, styleGenerator.BackgroundColor);
            Add(ForegroundColor, styleGenerator.ForegroundColor);

            Add(UIElemColor, styleGenerator.UIElemColor);

            Add(PositiveColor, styleGenerator.PositiveColor);
            Add(NegativeColor, styleGenerator.NegativeColor);

            Add(CornerRadius, styleGenerator.StyleSizes.CornerRadius);

            Add(TextSize_Text, styleGenerator.StyleSizes.TextSize_Text);
            Add(TextSize_Title, styleGenerator.StyleSizes.TextSize_Title);

            Add(LaTeXSize_Text, styleGenerator.StyleSizes.LaTeXSize_Text);
            Add(LaTeXSize_Title, styleGenerator.StyleSizes.LaTeXSize_Title);

            Add(OxyPlotChart_HeightScale, styleGenerator.StyleSizes.OxyPlotChart_HeightScale);
            InitializeComponent();
        }
    }
}