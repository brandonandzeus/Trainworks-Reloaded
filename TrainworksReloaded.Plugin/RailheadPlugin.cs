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
using TrainworksReloaded.Base.Card;

namespace TrainworkReloaded.Plugin
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new("TrainworksReloaded");

        private void Awake()
        {
            // Setup Game Client
            var client = new GameDataClient();
            DepInjector.AddClient(client);

            // Plugin startup logic
            Logger = base.Logger;

            //everything rail head
            var builder = Railhead.GetBuilder();
            builder.Configure(MyPluginInfo.PLUGIN_GUID, c =>
            {
                c.AddJsonFile("plugin.json");
            });

            Railend.ConfigurePreAction(c =>
            {
                //Register for Logging
                c.RegisterInstance<ManualLogSource>(Logger);

                //Register hooking into games dependency injection system
                c.RegisterInstance<GameDataClient>(client);

                //Register Card Data
                c.Register<IRegister<CardData>, CardDataRegister>(); //lookup for all registered card data
                c.Register<CardDataRegister, CardDataRegister>();
                c.RegisterSingleton<ICustomRegister<CardData>, CustomCardDataRegister>(); //a place to register and access custom card data
                c.RegisterSingleton<CustomCardDataRegister, CustomCardDataRegister>();
                c.Register<IDataPipeline<ICustomRegister<CardData>>, CardDataPipeline>(); //a data pipeline to run as soon as register is needed
                c.RegisterInitializer<ICustomRegister<CardData>>(x =>
                {
                    var pipeline = c.GetInstance<IDataPipeline<ICustomRegister<CardData>>>();
                    pipeline.Run(x);
                });

                //Register Loggers
                c.RegisterConditional(typeof(IModLogger<>), typeof(ModLogger<>), c => !c.Handled);
            });

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}