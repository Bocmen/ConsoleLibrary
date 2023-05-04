using System;

using WorkInvoker.Abstract;
using WorkInvoker.Interfaces;
using WorkInvoker.Models;
using ConsoleLibrary.ConsoleExtensions;

namespace WorkInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LoaderWorkBaseAttribute : Attribute, IGetWorkInfo
    {
        public const string DefaultGroupName = "Без названия";

        public readonly string Title;
        public readonly string Description;
        public readonly string GroupName;

        public LoaderWorkBaseAttribute(string title, string description, string groupName = DefaultGroupName)
        {
            Title = title;
            Description = description;
            GroupName = groupName;
        }

        public WorkInfo GetWorkInfo(object @params) // TODO проверка Type что он наследует WorkBase
        {
            if (@params != null && @params is Type type)
            {
                return new WorkInfo(Title, Description, GroupName, type.FullName, (console, token) =>
                {
                    WorkBase laboratory = (WorkBase)Activator.CreateInstance(type);
                    laboratory.BindConsole(console.CreateDecorator());
                    return laboratory.Start(token);
                });
            }
            throw new ArgumentNullException(nameof(@params));
        }
    }
}
