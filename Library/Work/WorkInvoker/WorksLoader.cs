using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WorkInvoker.Interfaces;
using WorkInvoker.Models;

namespace WorkInvoker
{
    public static class WorksLoader
    {
        private static readonly List<Assembly> assemblies = new List<Assembly>();
        private static readonly List<WorkInfo> _works = new List<WorkInfo>();
        private static readonly List<Func<Type, WorkInfo>> _worksLoaderInType = new List<Func<Type, WorkInfo>>();

        public static IEnumerable<WorkInfo> Works => _works;
        public static event Action ListWorksNewLoaded;

        static WorksLoader()
        {
            _worksLoaderInType.Add((type) =>
            {
                var workGeter = type.GetCustomAttributes().FirstOrDefault(x => x is IGetWorkInfo);
                return workGeter == null ? null : ((IGetWorkInfo)workGeter).GetWorkInfo(type);
            });
        }

        public static void AppendWorks(Assembly assembly)
        {
            if (assemblies.Contains(assembly)) throw new Exception("Данная сборка уже загружалась");
            if (GetWorksAssemblery(_works, assembly))
                ListWorksNewLoaded?.Invoke();
        }
        private static bool GetWorksAssemblery(List<WorkInfo> works, Assembly assembly)
        {
            bool isUpdate = false;
            foreach (var type in assembly.GetTypes())
            {
                foreach (var loader in _worksLoaderInType)
                {
                    var result = loader(type);
                    if (result != null)
                    {
                        if (!works.Contains(result))
                        {
                            works.Add(result);
                            isUpdate = true;
                        }
                        break;
                    }
                }
            }
            return isUpdate;
        }
        public static void AddLoaderWorks(Func<Type, WorkInfo> loaderWork)
        {
            if (_worksLoaderInType.Contains(loaderWork)) throw new Exception("Данная функция получения WorkInfo уже существует");
            _worksLoaderInType.Add(loaderWork);
            bool isUpdate = false;
            foreach (var assemblery in assemblies)
                isUpdate = isUpdate || GetWorksAssemblery(_works, assemblery);
            if(isUpdate)
                ListWorksNewLoaded?.Invoke();
        }
    }
}
