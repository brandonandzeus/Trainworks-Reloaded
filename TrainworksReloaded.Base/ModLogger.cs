using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base
{
    public class ModLogger<T> : IModLogger<T>
    {
        private readonly ManualLogSource manualLogSource;

        public ModLogger(ManualLogSource manualLogSource)
        {
            this.manualLogSource = manualLogSource;
        }
        public void Log(Core.Interfaces.LogLevel level, object data)
        {
            manualLogSource.Log((BepInEx.Logging.LogLevel)(int)level, data);
        }
    }
}
