using System;
using System.Collections.Generic;
using System.Linq;
using WorkInvoker.Models;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkInvoker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewGroupWorks : ContentView
    {
        public static readonly string FrameExpanderStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static readonly string NameGroupExpanderStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static readonly string FrameNodeContentExpanderStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        public static readonly string TitleContentExpanderStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static readonly string DescriptionContentExpanderStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));
        public static readonly string ButtonContentExpanderStyle = ConsoleLibrary.Tools.Styles.CreateNameStyle(nameof(WorkInvoker));

        private Action<WorkInfo> _actionOpenWork;
        private readonly string _uniqueName;

        public ViewGroupWorks(string uniqueName)
        {
            _uniqueName = uniqueName;
            InitializeComponent();
        }

        public void BindActionOpenWork(Action<WorkInfo> actionOpenWork)
        {
            if (_actionOpenWork == null)
            {
                _actionOpenWork = actionOpenWork;
                return;
            }
            throw new Exception("Нельзя менять событие открытия работы");
        }

        public void SetDataGroup(IEnumerable<KeyValuePair<string, IEnumerable<WorkInfo>>> data)
        {
            BindingContext = data.Select(x => new ExpandData()
            {
                IsExpanded = GroupIsExpanded(x.Key),
                GroupName = x.Key,
                Items = x.Value
            });
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is WorkInfo data)
                _actionOpenWork?.Invoke(data);
        }
        private string CreateKeyGroupExpand(string groupName) => $"{_uniqueName}_{nameof(ViewGroupWorks)}_{groupName}";
        private bool GroupIsExpanded(string nameGroup) => Preferences.Get(CreateKeyGroupExpand(nameGroup), true);

        private struct ExpandData
        {
            public bool IsExpanded { get; set; }
            public string GroupName { get; set; }
            public IEnumerable<WorkInfo> Items { get; set; }
        }

        private void Expander_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Expander.IsExpandedProperty.PropertyName && sender is Expander expander && expander.BindingContext is ExpandData expandData)
                Preferences.Set(CreateKeyGroupExpand(expandData.GroupName), expander.IsExpanded);
        }
    }
}