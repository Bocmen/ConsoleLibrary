using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using WorkInvoker.Abstract;
using WorkInvoker.Attributes;
using ConsoleLibrary.ConsoleExtensions;
using ConsoleLibrary.Extensions;


namespace ConsoleLibraryExample.WorksDemo
{
    [LoaderWorkBase("Пример графика", "")]
    public class Chart : WorkBase
    {
        private static IFormatProvider formatterDouble = new NumberFormatInfo { NumberDecimalSeparator = "," };

        private async Task CourseDemo()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync($"http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={DateTimeToString(DateTime.Now.Date.AddDays(-365 * 15))}&date_req2={DateTimeToString(DateTime.Now.Date)}&VAL_NM_RQ=R01235"))
                {
                    using (StreamReader sr = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ValCurs));
                        var parseData = (ValCurs)serializer.Deserialize(sr);

                        var model = new PlotModel { Title = "Курс доллара за последние 15 лет" };
                        model.Axes.Add(new DateTimeAxis
                        {
                            Position = AxisPosition.Bottom,
                            Minimum = DateTimeAxis.ToDouble(ParseDateTime(parseData.DateRange1)),
                            Maximum = DateTimeAxis.ToDouble(ParseDateTime(parseData.DateRange2))
                        });

                        LineSeries series = new LineSeries();

                        foreach (var node in parseData.Record)
                            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(ParseDateTime(node.Date)), ParseDouble(node.Value)));
                        model.Series.Add(series);
                        await Console.DrawChartOxyPlot(model);
                    }
                }
            }
        }

        private Task FunctionsSeriesDemo()
        {
            var model = new PlotModel { Title = "Fun with Bats" };

            Func<double, double> batFn1 = (x) => 2 * Math.Sqrt(-Math.Abs(Math.Abs(x) - 1) * Math.Abs(3 - Math.Abs(x)) / ((Math.Abs(x) - 1) * (3 - Math.Abs(x)))) * (1 + Math.Abs(Math.Abs(x) - 3) / (Math.Abs(x) - 3)) * Math.Sqrt(1 - Math.Pow((x / 7), 2)) + (5 + 0.97 * (Math.Abs(x - 0.5) + Math.Abs(x + 0.5)) - 3 * (Math.Abs(x - 0.75) + Math.Abs(x + 0.75))) * (1 + Math.Abs(1 - Math.Abs(x)) / (1 - Math.Abs(x)));
            Func<double, double> batFn2 = (x) => -3 * Math.Sqrt(1 - Math.Pow((x / 7), 2)) * Math.Sqrt(Math.Abs(Math.Abs(x) - 4) / (Math.Abs(x) - 4));
            Func<double, double> batFn3 = (x) => Math.Abs(x / 2) - 0.0913722 * (Math.Pow(x, 2)) - 3 + Math.Sqrt(1 - Math.Pow((Math.Abs(Math.Abs(x) - 2) - 1), 2));
            Func<double, double> batFn4 = (x) => (2.71052 + (1.5 - .5 * Math.Abs(x)) - 1.35526 * Math.Sqrt(4 - Math.Pow((Math.Abs(x) - 1), 2))) * Math.Sqrt(Math.Abs(Math.Abs(x) - 1) / (Math.Abs(x) - 1)) + 0.9;

            model.Series.Add(new FunctionSeries(batFn1, -8, 8, 0.01));
            model.Series.Add(new FunctionSeries(batFn2, -8, 8, 0.01));
            model.Series.Add(new FunctionSeries(batFn3, -8, 8, 0.01));
            model.Series.Add(new FunctionSeries(batFn4, -8, 8, 0.01));

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, MaximumPadding = 0.1, MinimumPadding = 0.1 });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, MaximumPadding = 0.1, MinimumPadding = 0.1 });

            return Console.DrawChartOxyPlot(model);
        }
        private Task BarSeriesDemo()
        {
            var model = new PlotModel
            {
                Title = "BarSeries",
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0
            };

            var s1 = new BarSeries { Title = "Series 1", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
            s1.Items.Add(new BarItem { Value = 25 });
            s1.Items.Add(new BarItem { Value = 137 });
            s1.Items.Add(new BarItem { Value = 18 });
            s1.Items.Add(new BarItem { Value = 40 });

            var s2 = new BarSeries { Title = "Series 2", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
            s2.Items.Add(new BarItem { Value = 12 });
            s2.Items.Add(new BarItem { Value = 14 });
            s2.Items.Add(new BarItem { Value = 120 });
            s2.Items.Add(new BarItem { Value = 26 });

            var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
            categoryAxis.Labels.Add("Category A");
            categoryAxis.Labels.Add("Category B");
            categoryAxis.Labels.Add("Category C");
            categoryAxis.Labels.Add("Category D");
            var valueAxis = new LinearAxis { Position = AxisPosition.Bottom, MinimumPadding = 0, MaximumPadding = 0.06, AbsoluteMinimum = 0 };
            model.Series.Add(s1);
            model.Series.Add(s2);
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);

            return Console.DrawChartOxyPlot(model);
        }
        private Task ContourSeries()
        {
            var model = new PlotModel { Title = "ContourSeries" };

            double x0 = -3.1;
            double x1 = 3.1;
            double y0 = -3;
            double y1 = 3;

            //generate values
            Func<double, double, double> peaks = (x, y) => 3 * (1 - x) * (1 - x) * Math.Exp(-(x * x) - (y + 1) * (y + 1)) - 10 * (x / 5 - x * x * x - y * y * y * y * y) * Math.Exp(-x * x - y * y) - 1.0 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
            var xx = ArrayBuilder.CreateVector(x0, x1, 100);
            var yy = ArrayBuilder.CreateVector(y0, y1, 100);
            var peaksData = ArrayBuilder.Evaluate(peaks, xx, yy);

            var cs = new ContourSeries
            {
                Color = OxyColors.Black,
                LabelBackground = OxyColors.White,
                ColumnCoordinates = yy,
                RowCoordinates = xx,
                Data = peaksData
            };
            model.Series.Add(cs);

            return Console.DrawChartOxyPlot(model);
        }
        private Task StemSeries()
        {
            var model = new PlotModel { Title = "Trigonometric functions" };

            var start = -Math.PI;
            var end = Math.PI;
            var step = 0.1;
            int steps = (int)((Math.Abs(start) + Math.Abs(end)) / step);

            //generate points for functions
            var sinData = new DataPoint[steps];
            for (int i = 0; i < steps; ++i)
            {
                var x = (start + step * i);
                sinData[i] = new DataPoint(x, Math.Sin(x));
            }

            //sin(x)
            var sinStemSeries = new StemSeries
            {
                MarkerStroke = OxyColors.Green,
                MarkerType = MarkerType.Circle
            };
            sinStemSeries.Points.AddRange(sinData);

            model.Series.Add(sinStemSeries);
            return Console.DrawChartOxyPlot(model);
        }

        public override async Task Start(CancellationToken token)
        {
            var demoCourse = CourseDemo();
            await FunctionsSeriesDemo();
            await BarSeriesDemo();
            await ContourSeries();
            await StemSeries();
            await demoCourse;

            await Console.WriteLine($"И другие виды графиков подробнее {FormattedStringExtension.LinkPattern("тут.", "https://oxyplot.readthedocs.io/en/latest/models/series/index.html#tracker")}");
        }
        private static string DateTimeToString(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy");
        private static DateTime ParseDateTime(string value) => DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        private static double ParseDouble(string value) => double.Parse(value, formatterDouble);

        [XmlRoot(ElementName = "Record")]
        public class Record
        {

            [XmlElement(ElementName = "Nominal")]
            public int Nominal;

            [XmlElement(ElementName = "Value")]
            public string Value;

            [XmlAttribute(AttributeName = "Date")]
            public string Date;

            [XmlAttribute(AttributeName = "Id")]
            public string Id;

            [XmlText]
            public string text;
        }

        [XmlRoot(ElementName = "ValCurs")]
        public class ValCurs
        {

            [XmlElement(ElementName = "Record")]
            public List<Record> Record;

            [XmlAttribute(AttributeName = "ID")]
            public string ID;

            [XmlAttribute(AttributeName = "DateRange1")]
            public string DateRange1;

            [XmlAttribute(AttributeName = "DateRange2")]
            public string DateRange2;

            [XmlAttribute(AttributeName = "name")]
            public string name;

            [XmlText]
            public string text;
        }

    }
}
