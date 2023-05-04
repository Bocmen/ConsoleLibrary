using System.Reflection;

using WorkInvoker.Pages;

using Xamarin.Forms;

namespace ConsoleLibraryExample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            WorkInvoker.WorksLoader.AppendWorks(Assembly.GetAssembly(typeof(App)));
            SettingPage.ApplayThemes();
            MainPage = WorkInvoker.Pages.MainPage.CreateRootPage(new DefaultViewWorks()
            {
                Title = DefaultViewWorks.DefaultTitlePage
            }, new HistoryViewWorks()
            {
                Title = HistoryViewWorks.DefaultTitlePage
            }, new SettingPage()
            {
                Title = SettingPage.DefaultTitlePage
            });
        }
    }
}
