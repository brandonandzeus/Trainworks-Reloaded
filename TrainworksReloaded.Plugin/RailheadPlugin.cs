using BepInEx;
using BepInEx.Logging;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using System.Reflection;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;
using TrainworksReloaded.Base;
using TrainworksReloaded.Plugin;
using HarmonyLib;
using System;
using System.Linq;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace TrainworkReloaded.Plugin
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new("MySource");

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");


            //everything rail head
            var builder = Railhead.GetBuilder();
            builder.Configure(MyPluginInfo.PLUGIN_GUID, c =>
            {
                var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(basePath, "plugin.json");
                c.AddJsonFile(path);
            });

            builder.ConfigureLoaders(c =>
            {

            });

            Railend.ConfigurePreAction(c =>
            {
                c.RegisterInstance<ManualLogSource>(Logger);
                c.RegisterSingleton<IRegister<CardData>, CustomCardDataRegister>();
                c.RegisterSingleton<CustomCardDataRegister, CustomCardDataRegister>();
                c.Register<IDataPipeline<IRegister<CardData>>, CardDataPipeline>();
                c.RegisterInitializer<IRegister<CardData>>(x =>
                {
                    var pipeline = c.GetInstance<IDataPipeline<IRegister<CardData>>>();
                    pipeline.Run(x);
                });
            });

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            /*
            Logger.LogInfo("Loaded Assemblies:");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName))
            {
                Console.WriteLine($"{assembly.FullName} - Location: {assembly.Location}");
            }
            */
        }
    }
}