using System;
using BepInEx.Logging;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base
{
    public class ModLoggerWrapper(Type type, object data)
    {
        private readonly Type type = type;
        private readonly object data = data;

        public override string ToString()
        {
            return $"[{type.Name}] {data.ToString()}";
        }
    }

    public class ModLogger<T>(ManualLogSource manualLogSource) : IModLogger<T>
    {
        private readonly ManualLogSource manualLogSource = manualLogSource;

        public void Log(Core.Interfaces.LogLevel level, object data)
        {
            manualLogSource.Log(
                (BepInEx.Logging.LogLevel)(int)level,
                new ModLoggerWrapper(typeof(T), data)
            );
        }
    }
}
