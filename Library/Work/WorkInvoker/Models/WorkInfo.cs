using System;
using System.Threading;
using System.Threading.Tasks;

using ConsoleLibrary.Interfaces;

using Xamarin.Essentials;

namespace WorkInvoker.Models
{
    public class WorkInfo
    {
        public delegate Task InvokeWork(IConsole console, CancellationToken token);

        public string Title { get; private set; }
        public string Description { get; private set; }
        public string GroupName { get; private set; }
        public string Id { get; private set; }
        public DateTime? LastOpen { get; private set; }
        public event Action LastOpenUpdated;

        private readonly InvokeWork _invokeWork;

        public WorkInfo(string title, string description, string groupName, string id, InvokeWork invokeWork)
        {
            Title = title;
            Description = description;
            GroupName = groupName;
            Id = id;
            _invokeWork = invokeWork;
            LastOpen = GetLastOpen(Id);
        }

        public static DateTime? GetLastOpen(string key)
        {
            if (Preferences.ContainsKey(key))
                return Preferences.Get(key, DateTime.MinValue);
            return null;
        }

        public async Task Invoke(IConsole console, CancellationToken token)
        {
            var setDateTime = DateTime.Now;
            LastOpen = setDateTime;
            Preferences.Set(Id, setDateTime);
            var task = _invokeWork.Invoke(console, token);
            LastOpenUpdated?.Invoke();
            await task;
        }
    }
}
