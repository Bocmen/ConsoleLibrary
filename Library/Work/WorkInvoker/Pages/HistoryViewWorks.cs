using System;
using System.Collections.Generic;
using System.Linq;

using WorkInvoker.Models;

namespace WorkInvoker.Pages
{
    public class HistoryViewWorks : ViewGroupWorksPage
    {
        public const string DefaultTitlePage = "История открытия работ";

        public HistoryViewWorks() : base(BindType.UpdateColection | BindType.UpdateLastOpen, nameof(HistoryViewWorks)) { }

        /// <summary>
        /// TODO Следует протестировать
        /// </summary>
        private static readonly IntervalInfo[] intervals = new IntervalInfo[]
        {
            new IntervalInfo("День", () => (DateTime.Now.Date, DateTime.Now.Date)),
            new IntervalInfo("Неделя", () =>
            {
                if(DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                    return (DateTime.Now.Date.AddDays(-7), DateTime.Now.Date.AddDays(-1));
                return (DateTime.Now.Date.AddDays(-1 * ((6 + (int)DateTime.Now.DayOfWeek) % 7)), DateTime.Now.Date);
            }),
            new IntervalInfo("Месяц", () =>
            {
                return (DateTime.Now.Date.AddDays(DateTime.Now.Day), DateTime.Now.Date);
            }),
            new IntervalInfo("Давно или неопределенно", () => (DateTime.MinValue, DateTime.MaxValue)),
        };

        protected override IEnumerable<KeyValuePair<string, IEnumerable<WorkInfo>>> GetWorksGrouped()
        {
            List<WorkInfo>[] grouping = Enumerable.Range(0, intervals.Length).Select(x => new List<WorkInfo>()).ToArray();
            foreach (var work in WorksLoader.Works)
            {
                for (int i = 0; i < intervals.Length; i++)
                {
                    if (intervals[i].IsEntry((work.LastOpen ?? DateTime.MinValue).Date))
                    {
                        grouping[i].Add(work);
                        break;
                    }
                }
            }
            for (int i = 0; i < grouping.Length; i++)
            {
                if (grouping[i].Any())
                    yield return new KeyValuePair<string, IEnumerable<WorkInfo>>(intervals[i].Name, grouping[i].OrderByDescending(x => x.LastOpen));
            }
            yield break;
        }

        private struct IntervalInfo
        {
            public delegate (DateTime? startDate, DateTime? endDate) GetInterval();

            public string Name { get; private set; }
            private readonly GetInterval _fGetInterval;

            public IntervalInfo(string name, GetInterval fGet)
            {
                _fGetInterval = fGet ?? throw new ArgumentNullException(nameof(fGet));
                Name = name;
            }

            public bool IsEntry(DateTime dateTime)
            {
                (DateTime? startDate, DateTime? endDate) = _fGetInterval.Invoke();

                if (startDate != null && endDate != null)
                    return startDate <= dateTime && dateTime <= endDate;
                else if (startDate == null)
                    return dateTime <= endDate;
                else if (endDate != null)
                    return dateTime >= startDate;
                throw new Exception("Неопределённое состояние, нет информации об интервале");
            }
        }
    }
}
