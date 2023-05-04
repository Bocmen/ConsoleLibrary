using System.Collections.Generic;
using WorkInvoker.Models;
using WorkInvoker.Views;

using Xamarin.Forms;

namespace WorkInvoker.Pages
{
    public abstract class ViewGroupWorksPage : ContentPage
    {
        public static string PageStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        protected abstract IEnumerable<KeyValuePair<string, IEnumerable<WorkInfo>>> GetWorksGrouped();

        public ViewGroupWorks ViewGroupWorks { get; private set; }

        protected ViewGroupWorksPage(BindType bindType, string uniqueName)
        {
            SetDynamicResource(StyleProperty, PageStyle);
            ViewGroupWorks = new ViewGroupWorks(uniqueName);
            Init();
            if (bindType.HasFlag(BindType.UpdateColection))
            {
                WorksLoader.ListWorksNewLoaded += () =>
                {
                    if (bindType.HasFlag(BindType.UpdateLastOpen)) BindUpdateLastOpen();
                    ReloadDataWorks();
                };
            }
            if (bindType.HasFlag(BindType.UpdateLastOpen))
            {
                BindUpdateLastOpen();
            }
        }

        private void BindUpdateLastOpen()
        {
            foreach (var work in WorksLoader.Works)
                work.LastOpenUpdated += RaloadDataUseMainThread;
        }
        private void RaloadDataUseMainThread() => Device.InvokeOnMainThreadAsync(() => ReloadDataWorks());

        protected void ReloadDataWorks()
        {
            ViewGroupWorks.SetDataGroup(GetWorksGrouped());
        }

        protected virtual void Init()
        {
            Content = ViewGroupWorks;
            ReloadDataWorks();
        }
        [System.Flags]
        protected enum BindType
        {
            None,
            UpdateColection,
            UpdateLastOpen
        }
    }
}
