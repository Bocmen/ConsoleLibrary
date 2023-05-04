using ConsoleLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleLibrary.Tools;

using Xamarin.Forms;

namespace ConsoleLibrary.ConsoleExtensions
{
    public static class ConsoleTableExtension
    {
        public static readonly string ColorBorderTableGrid = Styles.CreateNameStyleConsoleLibrary();
        public static readonly string SizeBorderTableGrid = Styles.CreateNameStyleConsoleLibrary();

        private static void GenerateRowOrColumn<T>(IList<T> toAdd, IEnumerable<T> originalCollection, int countElem, Func<T> patternSeparation, Func<T> defaultPattern)
        {
            toAdd.Add(patternSeparation.Invoke());
            if (originalCollection != null && originalCollection.Any())
            {
                foreach (var elem in originalCollection)
                {
                    toAdd.Add(elem);
                    toAdd.Add(patternSeparation.Invoke());
                }
            }
            else
            {
                for (int i = 0; i < countElem; i++)
                {
                    toAdd.Add(defaultPattern.Invoke());
                    toAdd.Add(patternSeparation.Invoke());
                }
            }
        }
        private static void SetterBordersInGrid(IEnumerable<(int xMin, int yMin, int xMax, int yMax)> boxes, bool isReverseBoxes, int countI, int countJ, Grid grid, Func<int, int, int, View> createBorder)
        {
            bool ContainsBoxes(int row, int column)
            {
                if (isReverseBoxes) (row, column) = (column, row);
                foreach (var (xMin, yMin, xMax, yMax) in boxes)
                    if (xMin <= column && yMin <= row && xMax >= column && yMax >= row) return true;
                return false;
            }
            for (int r = countI - 2; r >= 0; r--)
            {
                int iCurrent = r * 2 + 2;
                int jStart = 1, jEnd = 1;
                bool isInsertBox = false;
                for (int c = 0; c < countJ; c++)
                {
                    int originalColumn = c * 2 + 1;
                    if (!ContainsBoxes(iCurrent, originalColumn))
                    {
                        if (!isInsertBox) jStart = originalColumn;
                        isInsertBox = true;
                        jEnd = originalColumn;
                    }
                    else if (isInsertBox)
                    {
                        grid.Children.Add(createBorder.Invoke(iCurrent, jStart, jEnd - jStart + 1));
                        isInsertBox = false;
                        jStart = originalColumn;
                    }
                }
                if (isInsertBox) grid.Children.Add(createBorder.Invoke(iCurrent, jStart, jEnd - jStart + 1));
            }
        }
        private static View CreateBoxView()
        {
            var result = new BoxView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Margin = 0,
                CornerRadius = 0,
                BackgroundColor = Color.Black
            };
            result.SetDynamicResource(VisualElement.BackgroundColorProperty, ColorBorderTableGrid);
            return result;
        }

        private static void GridAddElem(this Grid grid, View view, int row, int column, int rowSpan = 1, int columnSpan = 1)
        {
            Grid.SetRow(view, row);
            Grid.SetColumn(view, column);

            Grid.SetRowSpan(view, rowSpan);
            Grid.SetColumnSpan(view, columnSpan);
            grid.Children.Add(view);
        }
        public static View CreateTableUseGrid(IEnumerable<ViewInsertFullInfo> tableData, TableInfo? tableInfo = null)
        {
            var constBorderSize = Convert.ToDouble(Application.Current.Resources[SizeBorderTableGrid]);

            Grid grid = new Grid
            {
                ColumnSpacing = 0,
                RowSpacing = 0,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };
            int countRowMax = Math.Max(tableInfo?.RowDefinitions?.Count() ?? 0, tableData.Select(x => x.Row + x.ViewInsertSpan.RowSpan).Max());
            int countColumnMax = Math.Max(tableInfo?.ColumnDefinitions?.Count() ?? 0, tableData.Select(x => x.Column + x.ViewInsertSpan.ColumnSpan).Max());
            GridGenerateOptions generateOptions = tableInfo?.GenerateOptions ?? TableInfo.DefautGenerateOption;
            GenerateRowOrColumn(grid.RowDefinitions, tableInfo?.RowDefinitions, countRowMax, () => new RowDefinition() { Height = new GridLength(constBorderSize, GridUnitType.Absolute) }, () => new RowDefinition() { Height = generateOptions.HasFlag(GridGenerateOptions.RowAuto) ? GridLength.Auto : GridLength.Star });
            GenerateRowOrColumn(grid.ColumnDefinitions, tableInfo?.ColumnDefinitions, countColumnMax, () => new ColumnDefinition() { Width = new GridLength(constBorderSize, GridUnitType.Absolute) }, () => new ColumnDefinition() { Width = generateOptions.HasFlag(GridGenerateOptions.ColumnAuto) ? GridLength.Auto : GridLength.Star });

            foreach (var elem in tableData) grid.GridAddElem(elem.ViewInsertSpan.View, elem.TableRow, elem.TableColumn, elem.ViewInsertSpan.TableRowSpan, elem.ViewInsertSpan.TableColumnSpan);
            IEnumerable<(int xMin, int yMin, int xMax, int yMax)> boxes = tableData.Where(x => x.ViewInsertSpan.ColumnSpan > 1 || x.ViewInsertSpan.RowSpan > 1).Select(x => (x.TableColumn, x.TableRow, x.TableColumn + x.ViewInsertSpan.TableColumnSpan - 1, x.TableRow + x.ViewInsertSpan.TableRowSpan - 1));
            SetterBordersInGrid(boxes, false, countRowMax, countColumnMax, grid, (i, j, span) =>
            {
                View view = CreateBoxView();
                Grid.SetColumn(view, j);
                Grid.SetRow(view, i);
                Grid.SetColumnSpan(view, span);
                return view;
            });
            SetterBordersInGrid(boxes, true, countColumnMax, countRowMax, grid, (i, j, span) =>
            {
                View view = CreateBoxView();
                Grid.SetRow(view, j);
                Grid.SetColumn(view, i);
                Grid.SetRowSpan(view, span);
                return view;
            });
            // А теперь ещё не хватает внешних границ
            countRowMax *= 2;
            countColumnMax *= 2;
            grid.GridAddElem(CreateBoxView(), 0, 0, countRowMax + 1);
            grid.GridAddElem(CreateBoxView(), 0, countColumnMax, countRowMax + 1);
            grid.GridAddElem(CreateBoxView(), 0, 0, 1, countColumnMax + 1);
            grid.GridAddElem(CreateBoxView(), countRowMax, 0, 1, countColumnMax + 1);
            return grid;
        }
        public static View CreateTableUseGrid(IEnumerable<IEnumerable<ViewInsertSpanInfo>> tableData, TableInfo? tableInfo = null, bool isTransposition = false) => CreateTableUseGrid(tableData.SelectMany((x, i) => x.Select((y, j) => isTransposition ? y.CreateFullInfo(i, j) : y.CreateFullInfo(j, i))), tableInfo);
        public static View CreateTableUseGrid(IEnumerable<IEnumerable<object>> tableData, TableInfo? tableInfo = null, bool isTransposition = false) => CreateTableUseGrid(tableData.SelectMany((x, i) => x.Select((y, j) =>
        {
            if (y is ViewInsertFullInfo fullInfo)
            {
                return fullInfo;
            }
            else if (y is ViewInsertSpanInfo spanInfo)
            {
                return isTransposition ? spanInfo.CreateFullInfo(i, j) : spanInfo.CreateFullInfo(j, i);
            }
            else
            {
                View view;
                if (y is View viewSet) view = viewSet;
                else
                {
                    view = ConsoleIOExtension.CreateLabel(y.ToString());
                    view.HorizontalOptions = LayoutOptions.Center;
                    view.VerticalOptions = LayoutOptions.Center;
                }
                return new ViewInsertFullInfo()
                {
                    Column = isTransposition ? j : i,
                    Row = isTransposition ? i : j,
                    ViewInsertSpan = new ViewInsertSpanInfo()
                    {
                        View = view
                    }
                };
            }
        })), tableInfo);

        public static Task DrawTableUseGrid(this IConsole console, IEnumerable<ViewInsertFullInfo> tableData, TableInfo? tableInfo = null) => console.AddUIElement(CreateTableUseGrid(tableData, tableInfo));
        public static Task DrawTableUseGrid(this IConsole console, IEnumerable<IEnumerable<ViewInsertSpanInfo>> tableData, TableInfo? tableInfo = null, bool isTransposition = false) => console.AddUIElement(CreateTableUseGrid(tableData, tableInfo, isTransposition));
        public static Task DrawTableUseGrid(this IConsole console, IEnumerable<IEnumerable<object>> tableData, TableInfo? tableInfo = null, bool isTransposition = false) => console.AddUIElement(CreateTableUseGrid(tableData, tableInfo, isTransposition));


        public struct TableInfo
        {
            public static GridGenerateOptions DefautGenerateOption = GridGenerateOptions.None;

            public GridGenerateOptions? GenerateOptions { get; private set; }
            public IEnumerable<RowDefinition> RowDefinitions { get; private set; }
            public IEnumerable<ColumnDefinition> ColumnDefinitions { get; private set; }

            public TableInfo(GridGenerateOptions generateOptions)
            {
                GenerateOptions = generateOptions;
                RowDefinitions = null;
                ColumnDefinitions = null;
            }
            public TableInfo(IEnumerable<RowDefinition> rowDefinitions, IEnumerable<ColumnDefinition> columnDefinitions)
            {
                RowDefinitions = rowDefinitions;
                ColumnDefinitions = columnDefinitions;
                GenerateOptions = null;
            }
        }
        public struct ViewInsertSpanInfo
        {
            public View View { get; set; }
            public object SetViewAutoDetect
            {
                set
                {
                    if (value is View view)
                    {
                        View = view;
                    }
                    else
                    {
                        View = ConsoleIOExtension.CreateLabel(value.ToString());
                        View.HorizontalOptions = LayoutOptions.Center;
                        View.VerticalOptions = LayoutOptions.Center;
                    }
                }
            }
            public int RowSpan
            {
                get => _rowSpan > 0 ? _rowSpan : 1;
                set => _rowSpan = value;
            }
            public int ColumnSpan
            {
                get => _columnSpan > 0 ? _columnSpan : 1;
                set => _columnSpan = value;
            }
            public int TableRowSpan => RowSpan * 2 - 1;
            public int TableColumnSpan => ColumnSpan * 2 - 1;

            private int _rowSpan;
            private int _columnSpan;

            public static implicit operator ViewInsertSpanInfo(string text) => new ViewInsertSpanInfo()
            {
                SetViewAutoDetect = text
            };
            public static implicit operator ViewInsertSpanInfo(View view) => new ViewInsertSpanInfo()
            {
                View = view
            };

            public ViewInsertFullInfo CreateFullInfo(int row, int column) => new ViewInsertFullInfo()
            {
                Row = row,
                Column = column,
                ViewInsertSpan = this
            };
        }
        public struct ViewInsertFullInfo
        {
            public int Row;
            public int Column;
            public ViewInsertSpanInfo ViewInsertSpan;

            public int TableRow => Row * 2 + 1;
            public int TableColumn => Column * 2 + 1;
        }

        [Flags]
        public enum GridGenerateOptions : byte
        {
            None,
            ColumnAuto,
            RowAuto
        }
    }
}
