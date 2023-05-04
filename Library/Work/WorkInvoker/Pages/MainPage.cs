using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WorkInvoker.Models;
using ConsoleLibrary.ConsoleExtensions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkInvoker.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public static string PageStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        private CancellationTokenSource _tokenOpenWork;
        private Page _pageOpenWork;

        private MainPage(Page[] pages)
        {
            SetDynamicResource(StyleProperty, PageStyle);
            foreach (var page in pages)
            {
                Children.Add(page);
                if (page is ViewGroupWorksPage groupWorksPage)
                    groupWorksPage.ViewGroupWorks.BindActionOpenWork(OpenWork);
            }
        }
        private async void Current_PageDisappearing(object sender, Page e)
        {
            if (e == this) await WorkClose();
        }
        public static Page CreateRootPage(params Page[] pages) => new NavigationPage(new MainPage(pages));
        private async void OpenWork(WorkInfo workInfo)
        {
            Application.Current.PageAppearing -= Current_PageDisappearing;
            await WorkClose();
            var console = new ConsoleLibrary.Views.Console();
            _pageOpenWork = new ContentPage()
            {
                Content = console,
                Title = workInfo.Title
            };
            _tokenOpenWork = new CancellationTokenSource();
            await Application.Current.MainPage.Navigation.PushAsync(_pageOpenWork);
            Application.Current.PageAppearing += Current_PageDisappearing;
            _ = Task.Run(async () =>
            {
                var consoleDecorator = console.CreateDecorator();
                consoleDecorator.OffDecoration();
                try
                {
                restart:
                    await workInfo.Invoke(console, _tokenOpenWork.Token);
                    consoleDecorator.SetDecorationOneElem();
                    if (await consoleDecorator.ReadBool("Перезапустить работу?", token: _tokenOpenWork.Token))
                    {
                        consoleDecorator.OffDecoration();
                        await consoleDecorator.Clear();
                        goto restart;
                    }
                }
                catch (Exception e)
                {
                    consoleDecorator.StartCollectionDecorate();
                    await consoleDecorator.DrawSeparatorLine();
                    await consoleDecorator.WriteLine("Возникла ошибка при работе", ConsoleIOExtension.TextStyle.IsError | ConsoleIOExtension.TextStyle.IsTitle);
                    await consoleDecorator.DrawSeparatorLine();
                    await consoleDecorator.WriteLine(e.Message);
                    await consoleDecorator.WriteLine("Стек вызова:", ConsoleIOExtension.TextStyle.IsError);
                    await consoleDecorator.WriteLine(e.StackTrace);
                    consoleDecorator.OffDecoration();
                }
            });
        }
        private async Task WorkClose()
        {
            if (_tokenOpenWork != null)
            {
                _tokenOpenWork.Cancel();
                _tokenOpenWork.Dispose();
                _tokenOpenWork = null;
            }
            if (_pageOpenWork == Application.Current.MainPage.Navigation.NavigationStack.Last())
                await Application.Current.MainPage.Navigation.PopAsync(false);
        }
    }
}