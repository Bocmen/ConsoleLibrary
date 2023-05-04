using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using Xamarin.Forms;

using static ConsoleLibrary.ConsoleExtensions.ConsoleTableExtension;
using ConsoleLibrary.ConsoleExtensions;
using ConsoleLibrary.Extensions;
using WorkInvoker.Abstract;
using WorkInvoker.Attributes;
using ConsoleLibraryExample.Resources;

namespace ConsoleLibraryExample.WorksDemo
{
    [LoaderWorkBase("Текстовый вывод", "")]
    public class Output : WorkBase
    {
        private static readonly Random random = new Random();
        private const string NameFileOne = "OutputDemoTextOne.txt";
        private const string NameFileTwo = "OutputDemoTextTwo.txt";

        public override async Task Start(CancellationToken token)
        {
            var timeViewEditor = await Console.WriteLineReturnEditor("", ConsoleIOExtension.TextStyle.IsTitle);
            _ = Task.Run(() =>
            {
                TimeSpan dateTime = DateTime.Now.TimeOfDay;
                while (!token.IsCancellationRequested)
                {
                    var deltaTime = DateTime.Now.TimeOfDay - dateTime;
                    timeViewEditor.SetContent(FormattedStringExtension.ColorPattern($"Прошло время: {deltaTime}", Color.FromRgb(random.Next(256), random.Next(256), random.Next(256))));
                    Thread.Sleep(80);
                }
            });
            Console.StartCollectionDecorate();
            await Console.WriteLine("IsTitle[false], IsError[false]: \\( \\frac{\\sqrt a}{b} \\)", ConsoleIOExtension.TextStyle.UseLaTeX);
            await Console.WriteLine("IsTitle[false], IsError[true]: \\( \\frac{\\sqrt a}{b} \\)", ConsoleIOExtension.TextStyle.UseLaTeX | ConsoleIOExtension.TextStyle.IsError);
            await Console.WriteLine("IsTitle[true], IsError[false]: \\( \\frac{\\sqrt a}{b} \\)", ConsoleIOExtension.TextStyle.UseLaTeX | ConsoleIOExtension.TextStyle.IsTitle);
            await Console.WriteLine("IsTitle[true], IsError[true]: \\( \\frac{\\sqrt a}{b} \\)", ConsoleIOExtension.TextStyle.UseLaTeX | ConsoleIOExtension.TextStyle.IsTitle | ConsoleIOExtension.TextStyle.IsError);
            Console.StartCollectionDecorate();
            await Console.WriteLine("IsTitle[false], IsError[false]: Обычный текст");
            await Console.WriteLine("IsTitle[false], IsError[true]: Обычный текст", ConsoleIOExtension.TextStyle.IsError);
            await Console.WriteLine("IsTitle[true], IsError[false]: Обычный текст", ConsoleIOExtension.TextStyle.IsTitle);
            await Console.WriteLine("IsTitle[true], IsError[true]: Обычный текст", ConsoleIOExtension.TextStyle.IsTitle | ConsoleIOExtension.TextStyle.IsError);
            Console.StartCollectionDecorate();
            await Console.WriteLine("Пример форматированного текста", ConsoleIOExtension.TextStyle.IsTitle);
            await Console.WriteLine($"Пример ссылок: {FormattedStringExtension.LinkPattern("Автор библиотеки", "https://vk.com/bocmenden")}\nПример окрашивания текста: {FormattedStringExtension.ColorPattern("Красный", Color.Red)}");
            Console.SetDecorationOneElem();

            var poemOneView = ConsoleIOExtension.CreateLabel(string.Empty);
            poemOneView.HorizontalOptions = LayoutOptions.Fill;
            poemOneView.SetValue(Label.HorizontalTextAlignmentProperty, TextAlignment.Center);

            var poemTwoView = ConsoleIOExtension.CreateLabel(string.Empty);
            poemTwoView.HorizontalOptions = LayoutOptions.Fill;
            poemTwoView.SetValue(Label.HorizontalTextAlignmentProperty, TextAlignment.Center);

            await Console.DrawTableUseGrid(new List<ViewInsertFullInfo>()
            {
                new ViewInsertFullInfo()
                {
                    Row = 0,
                    Column = 0,
                    ViewInsertSpan = new ViewInsertSpanInfo()
                    {
                        View = poemOneView
                    }
                },
                new ViewInsertFullInfo()
                {
                    Row = 0,
                    Column = 1,
                    ViewInsertSpan = new ViewInsertSpanInfo()
                    {
                        View = poemTwoView
                    }
                }
            });

            var oneTextEditor = new ConsoleIOExtension.LabelEditor(poemOneView, Console.InvockeUITheardAction);
            var poemTwoEditor = new ConsoleIOExtension.LabelEditor(poemTwoView, Console.InvockeUITheardAction);

            var taskTextWriterOne = DynamicWriterTextLabelInFile(oneTextEditor, NameFileOne, token);
            var taskTextWriterTwo = DynamicWriterTextLabelInFile(poemTwoEditor, NameFileTwo, token);
            await taskTextWriterOne;
            await taskTextWriterTwo;
        }
        private Task DynamicWriterTextLabelInFile(ConsoleIOExtension.LabelEditor labelEditor, string path, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                using (var reader = ResourceTextReader.GetReader(path))
                {
                    StringBuilder textFile = new StringBuilder();
                    while (!reader.EndOfStream && !token.IsCancellationRequested)
                    {
                        string line = reader.ReadLine();
                        foreach (var @char in line)
                        {
                            textFile.Append(@char);
                            await labelEditor.SetContent(textFile.ToString());
                            Thread.Sleep(100);
                        }
                        textFile.Append("\n");
                    }
                }
            });
        }
    }
}
