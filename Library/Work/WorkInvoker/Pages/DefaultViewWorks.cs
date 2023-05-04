using System;
using System.Collections.Generic;
using System.Linq;

using WorkInvoker.Models;

namespace WorkInvoker.Pages
{
    public class DefaultViewWorks : ViewGroupWorksPage
    {
        public const string DefaultTitlePage = "Группы работ";

        public DefaultViewWorks() : base(BindType.UpdateColection | BindType.UpdateLastOpen, nameof(DefaultViewWorks)) { }

        protected override IEnumerable<KeyValuePair<string, IEnumerable<WorkInfo>>> GetWorksGrouped()
        {
            var r = WorksLoader.Works;
            var t = r.GroupBy(x => x.GroupName);
            var c = t.Select(x => (x.Key, x.OrderByDescending(d => d.LastOpen)));
            var o = c.OrderByDescending(x =>
            {
                var seq = x.Item2.Where(l => l.LastOpen != null);
                if (!seq.Any()) return DateTime.MinValue;
                return seq.Max(l => l.LastOpen);
            });
            return o.Select(x => new KeyValuePair<string, IEnumerable<WorkInfo>>(x.Key, x.Item2));
        }
    }
}
