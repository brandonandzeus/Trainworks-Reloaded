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
using UnityEngine;
using TrainworksReloaded.Base.Prefab;
using UnityEngine.AddressableAssets;
using TrainworksReloaded.Base.Trait;

namespace TrainworkReloaded.Plugin
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new("TrainworksReloaded");

        internal static Lazy<Container> Container = new(() => Railend.GetContainer());

        public void Awake()
        {
            //Pregame Actions
            var configToolsCSV = Config.Bind("Tools", "Generate CSVs", false, "Enable to Generate the Games' CSV Files on Launch");
            if (configToolsCSV.Value)
            {
                var basePath = Path.GetDirectoryName(typeof(Plugin).Assembly.Location);
                LocalizationManager.InitializeIfNeeded();
                for (int i = 0; i < LocalizationManager.Sources.Count; i++)
                {
                    var source = LocalizationManager.Sources[i];
                    foreach (var category in source.GetCategories())
                    {
                        var str = source.Export_CSV(category);
                        var file_name = $"{i}_{category}.csv";
                        File.WriteAllText(Path.Combine(basePath, file_name), str);
                    }
                }
            }


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
                c.RegisterSingleton<IRegister<CardData>, CardDataRegister>(); //a place to register and access custom card data
                c.RegisterSingleton<CardDataRegister, CardDataRegister>();
                c.Register<IDataPipeline<IRegister<CardData>>, CardDataPipeline>(); //a data pipeline to run as soon as register is needed
                c.RegisterInitializer<IRegister<CardData>>(x =>
                {
                    var pipeline = c.GetInstance<IDataPipeline<IRegister<CardData>>>();
                    pipeline.Run(x);
                });

                //Register Class Data
                c.RegisterSingleton<IRegister<ClassData>, ClassDataRegister>();
                c.RegisterSingleton<ClassDataRegister, ClassDataRegister>();

                //Register Trait Data
                c.RegisterSingleton<IRegister<CardTraitData>, CardTraitDataRegister>();
                c.RegisterSingleton<CardTraitDataRegister, CardTraitDataRegister>();
                c.Register<IDataPipeline<IRegister<CardTraitData>>, CardTraitDataPipeline>(); 
                c.RegisterInitializer<IRegister<CardTraitData>>(x =>
                {
                    var pipeline = c.GetInstance<IDataPipeline<IRegister<CardTraitData>>>();
                    pipeline.Run(x);
                });

                //Register Localization
                c.RegisterSingleton<IRegister<LocalizationTerm>, CustomLocalizationTermRegistry>();
                c.RegisterSingleton<CustomLocalizationTermRegistry, CustomLocalizationTermRegistry>();

                //Register Assets
                c.RegisterSingleton<IRegister<GameObject>, GameobjectRegister>();
                c.RegisterSingleton<GameobjectRegister, GameobjectRegister>();
                c.RegisterInitializer<GameobjectRegister>(x =>
                {
                    Addressables.ResourceLocators.Add(x);
                });
                c.Register<IDataPipeline<IRegister<GameObject>>, TextureImportPipeline>(); //a data pipeline to run as soon as register is needed
                c.RegisterInitializer<IRegister<GameObject>>(x =>
                {
                    var pipeline = c.GetInstance<IDataPipeline<IRegister<GameObject>>>();
                    pipeline.Run(x);
                });


                //Register Loggers
                c.RegisterSingleton<IModLogger<CustomLocalizationTermRegistry>, ModLogger<CustomLocalizationTermRegistry>>();
                c.RegisterConditional(typeof(IModLogger<>), typeof(ModLogger<>), c => !c.Handled);
            });


            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}