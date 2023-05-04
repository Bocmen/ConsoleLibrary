using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;

using ConsoleLibrary.ConsoleExtensions;
using static ConsoleLibrary.ConsoleExtensions.ConsoleIOExtension;
using static WorkInvoker.Styles.Themes;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkInvoker.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        private static string LightStyleKey = $"{nameof(SettingPage)}_{nameof(LightStyle)}";
        private static string DarkStyleKey = $"{nameof(SettingPage)}_{nameof(DarkStyle)}";

        public const string DefaultTitlePage = "Настройки оформления";

        public static readonly string PageStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static readonly string LabelStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static readonly string FrameDecorationStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static readonly string ButtonStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static readonly string SliderStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static StyleGenerator LightStyle { get; private set; }
        public static StyleGenerator DarkStyle { get; private set; }

        public SettingPage()
        {
            InitializeComponent();
            TitleThemeCurrent.Text = GetTextTitleTheme(CurrentTheme);
            InitExampleConsole();
        }
        private static string GetTextTitleTheme(ThemeType themeType) => $"Хотите сменить тему на {(themeType == ThemeType.Light ? "тёмную" : "светлую")}?";

        static SettingPage()
        {
            LightStyle = LoadStyleGenerator(LightStyleKey) ?? DefaultLightStyleGenerator;
            DarkStyle = LoadStyleGenerator(DarkStyleKey) ?? DefaultDarkStyleGenerator;
        }

        private static StyleGenerator? LoadStyleGenerator(string key)
        {
            return null;
        }

        public static void ApplayThemes()
        {
            EditTheme(CreateStyles(LightStyle), ThemeType.Light);
            EditTheme(CreateStyles(DarkStyle), ThemeType.Dark);
        }

        private void ChangeTheme_Clicked(object sender, EventArgs e)
        {
            SetTheme(CurrentTheme == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark);
            TitleThemeCurrent.Text = GetTextTitleTheme(CurrentTheme);
        }


        private async void InitExampleConsole()
        {
            var Console = this.Console.CreateDecorator();
        restart:
            Console.StartCollectionDecorate();
            await Console.WriteLine("IsTitle[false], IsError[false]: \\( \\frac{\\sqrt a}{b} \\)", TextStyle.UseLaTeX);
            await Console.WriteLine("IsTitle[false], IsError[true]: \\( \\frac{\\sqrt a}{b} \\)", TextStyle.UseLaTeX | TextStyle.IsError);
            await Console.WriteLine("IsTitle[true], IsError[false]: \\( \\frac{\\sqrt a}{b} \\)", TextStyle.UseLaTeX | TextStyle.IsTitle);
            await Console.WriteLine("IsTitle[true], IsError[true]: \\( \\frac{\\sqrt a}{b} \\)", TextStyle.UseLaTeX | TextStyle.IsTitle | TextStyle.IsError);
            Console.StartCollectionDecorate();
            await Console.WriteLine("IsTitle[false], IsError[false]: Обычный текст");
            await Console.WriteLine("IsTitle[false], IsError[true]: Обычный текст", TextStyle.IsError);
            await Console.WriteLine("IsTitle[true], IsError[false]: Обычный текст", TextStyle.IsTitle);
            await Console.WriteLine("IsTitle[true], IsError[true]: Обычный текст", TextStyle.IsTitle | TextStyle.IsError);
            Console.SetDecorationOneElem();
            _ = Console.ReadLine("Пример ввода", validF: (t) => ResultValidate.CreateError("Пример неверного ввода"));
            Console.StartCollectionDecorate();
            await Console.WriteLine("Пример таблицы", TextStyle.IsTitle);
            await Console.DrawTableUseGrid(new List<List<object>>()
            {
                new List<object>
                {
                    new Frame() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.GreenYellow, CornerRadius = 0 },
                    "Вставка текста",
                    1.555555
                },
                new List<object>
                {
                    "Вставка текста",
                    new Frame() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.LightGoldenrodYellow, CornerRadius = 0 },
                    1.555555
                },
                new List<object>
                {
                    "Вставка текста",
                    1.555555,
                    new Frame() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.Yellow, CornerRadius = 0 },
                }
            });
            Console.StartCollectionDecorate();
            await Console.WriteLine("Пример графика", TextStyle.IsTitle);
            await Console.DrawChartOxyPlot(new PlotModel()
            {
                Title = "Заголовок",
                Series =
                {
                    new FunctionSeries(Math.Cos, -20, 20,  0.05)
                }
            });
            Console.SetDecorationOneElem();
            if (await Console.ReadBool("Перезапустить консоль?"))
            {
                await Console.Clear();
                goto restart;
            }
        }
    }
}