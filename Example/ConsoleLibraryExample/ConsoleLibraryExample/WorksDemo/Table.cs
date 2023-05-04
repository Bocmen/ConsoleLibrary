using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Xamarin.Forms;

using WorkInvoker.Abstract;
using WorkInvoker.Attributes;
using static ConsoleLibrary.ConsoleExtensions.ConsoleTableExtension;
using ConsoleLibrary.ConsoleExtensions;
using ConsoleLibrary.Extensions;

namespace ConsoleLibraryExample.WorksDemo
{
    [LoaderWorkBase("Пример использования таблиц", "")]
    public class Table : WorkBase
    {
        private static IEnumerable<ViewInsertFullInfo> CreateDemoData() => new List<ViewInsertFullInfo>()
        {
            new ViewInsertFullInfo()
            {
                Row = 0,
                Column = 0,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 0,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 0,
                Column = 1,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 1,
                    RowSpan = 1,
                    ColumnSpan = 3
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 0,
                Column = 4,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 2,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 1,
                Column = 4,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 3,
                    RowSpan = 3,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 4,
                Column = 4,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 4,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 4,
                Column = 1,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 5,
                    RowSpan = 1,
                    ColumnSpan = 3
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 4,
                Column = 0,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 6,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 1,
                Column = 0,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 7,
                    RowSpan = 3,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 1,
                Column = 1,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 8,
                    RowSpan = 1,
                    ColumnSpan = 2
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 1,
                Column = 3,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 9,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 2,
                Column = 3,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 10,
                    RowSpan = 2,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 3,
                Column = 2,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 11,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 3,
                Column = 1,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 12,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 2,
                Column = 1,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 13,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
            new ViewInsertFullInfo()
            {
                Row = 2,
                Column = 2,
                ViewInsertSpan = new ViewInsertSpanInfo()
                {
                    SetViewAutoDetect = 14,
                    RowSpan = 1,
                    ColumnSpan = 1
                }
            },
        };

        public override async Task Start(CancellationToken token)
        {
            var p1 = new GridLength(1.0 / 3, GridUnitType.Star);
            var p2 = new GridLength(1.0 / 6, GridUnitType.Star);
            var p3 = new GridLength(1.0 / 12, GridUnitType.Star);
            Console.StartCollectionDecorate();
            await Console.WriteLine("Пример таблицы с полным указанием данных о размерах строк и столбцов", ConsoleIOExtension.TextStyle.IsTitle);
            await Console.DrawTableUseGrid(CreateDemoData(), new TableInfo(new List<RowDefinition>()
            {
                new RowDefinition() { Height = p1 },
                new RowDefinition() { Height = p2 },
                new RowDefinition() { Height = p3 },
                new RowDefinition() { Height = p3 },
                new RowDefinition() { Height = p1 }
            },
            new List<ColumnDefinition>()
            {
                new ColumnDefinition() { Width = p1 },
                new ColumnDefinition() { Width = p3 },
                new ColumnDefinition() { Width = p3 },
                new ColumnDefinition() { Width = p2 },
                new ColumnDefinition() { Width = p1 }
            }));
            Console.StartCollectionDecorate();
            await Console.WriteLine("Пример таблицы без указаний данных\n(параметры генерации строк и столбцов: Star)", ConsoleIOExtension.TextStyle.IsTitle);
            await Console.DrawTableUseGrid(CreateDemoData());
            Console.StartCollectionDecorate();
            await Console.WriteLine("Пример таблицы без указаний данных\n(параметры генерации строк и столбцов: Auto)", ConsoleIOExtension.TextStyle.IsTitle);
            await Console.DrawTableUseGrid(CreateDemoData(), new TableInfo(GridGenerateOptions.ColumnAuto | GridGenerateOptions.RowAuto));
            Console.StartCollectionDecorate();
            await Console.WriteLine("Не рекомендуется вставлять таблицу в таблицу", ConsoleIOExtension.TextStyle.IsTitle);
            await Console.AddUIElement(CreateTableUseGrid(new List<List<object>>()
            {
                new List<object>
                {
                    new Frame() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.GreenYellow, CornerRadius = 0 },
                    "Вставка текста",
                    CreateTableUseGrid(new List<List<object>>
                    {
                        new List<object>
                        {
                            FormattedStringExtension.ColorPattern("Плохо", Color.Red),
                            FormattedStringExtension.ColorPattern("Плохо", Color.Red),
                        },
                        new List<object>
                        {
                            FormattedStringExtension.ColorPattern("Плохо", Color.Red),
                            CreateTableUseGrid(new List<List<object>>
                            {
                                new List<object>
                                {
                                    FormattedStringExtension.ColorPattern("Плохо", Color.Red),
                                    FormattedStringExtension.ColorPattern("Плохо", Color.Red),
                                },
                                new List<object>
                                {
                                    FormattedStringExtension.ColorPattern("Плохо", Color.Red),
                                    FormattedStringExtension.ColorPattern("Плохо", Color.Red),
                                }
                            })
                        }
                    })
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
            }));
        }
    }
}
