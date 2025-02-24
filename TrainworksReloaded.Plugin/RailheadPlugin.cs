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
using I2.Loc;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Base.Class;

namespace TrainworkReloaded.Plugin
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new("TrainworksReloaded");

        public void Awake()
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

                //Register Class Data
                c.Register<IRegister<ClassData>, ClassDataRegister>();
                c.Register<ClassDataRegister, ClassDataRegister>();

                //Register Localization
                c.RegisterSingleton<ICustomRegister<LocalizationTerm>, CustomLocalizationTermRegistry>();
                c.RegisterSingleton<CustomLocalizationTermRegistry, CustomLocalizationTermRegistry>();

                //Register Loggers
                c.RegisterSingleton<IModLogger<CustomLocalizationTermRegistry>, ModLogger<CustomLocalizationTermRegistry>>();
                c.RegisterConditional(typeof(IModLogger<>), typeof(ModLogger<>), c => !c.Handled);
            });

            //var basePath = Path.GetDirectoryName(typeof(Plugin).Assembly.Location);
            //LocalizationManager.InitializeIfNeeded();
            //for (int i = 0; i < LocalizationManager.Sources.Count; i++)
            //{
            //    var source = LocalizationManager.Sources[i];
            //    foreach (var category in source.GetCategories())
            //    {
            //        var str = source.Export_CSV(category);
            //        var file_name = $"{i}_{category}.csv";
            //        File.WriteAllText(Path.Combine(basePath, file_name), str);
            //    }
            //}

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}