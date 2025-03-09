using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Moq;
using SimpleInjector;
using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using TrainworksReloaded.Plugin;
using UnityEngine;

namespace TrainworksReloaded.Test
{
    public class CardTests : IDisposable
    {
        public Container Container { get; set; }
        public Guid TestGuid { get; set; }
        public Dictionary<string, LocalizationTerm> TermDictionary { get; set; }
        public List<(LogLevel Level, object Message)> LoggedMessages { get; set; }

        public CardTests()
        {
            Container = new Container();

            //Atlas
            var atlas = new PluginAtlas();
            var configuration = new ConfigurationBuilder();
            var basePath = Path.GetDirectoryName(this.GetType().Assembly.Location);
            configuration.SetBasePath(basePath!);
            configuration.AddJsonFile("examples/cards/fire_starter.json");
            var definition = new PluginDefinition(configuration.Build());
            atlas.PluginDefinitions.Add("MyMod", definition);
            Container.RegisterInstance<PluginAtlas>(atlas);

            //Guid
            TestGuid = new Guid();
            var guidProvider = new Mock<IGuidProvider>();
            guidProvider.Setup(xs => xs.GetGuidDeterministic(It.IsAny<string>())).Returns(TestGuid);
            Container.RegisterInstance<IGuidProvider>(guidProvider.Object);

            //Instance Generator
            Container.RegisterConditional(
                typeof(IInstanceGenerator<>),
                typeof(InstanceGenerator<>),
                c => !c.Handled
            );

            // Term Register
            TermDictionary = new Dictionary<string, LocalizationTerm>();
            var termRegister = new Mock<IRegister<LocalizationTerm>>();

            // Register Method
            termRegister
                .Setup(tr => tr.Register(It.IsAny<string>(), It.IsAny<LocalizationTerm>()))
                .Callback<string, LocalizationTerm>((key, term) => TermDictionary[key] = term);

            // TryLookupName
            termRegister
                .Setup(tr =>
                    tr.TryLookupName(
                        It.IsAny<string>(),
                        out It.Ref<LocalizationTerm?>.IsAny,
                        out It.Ref<bool?>.IsAny
                    )
                )
                .Returns(
                    (string name, out LocalizationTerm? term, out bool? isModded) =>
                    {
                        term = TermDictionary.Values.FirstOrDefault(t => t.English == name);
                        isModded = false;
                        return term != null;
                    }
                );

            // TryLookupId
            termRegister
                .Setup(tr =>
                    tr.TryLookupId(
                        It.IsAny<string>(),
                        out It.Ref<LocalizationTerm?>.IsAny,
                        out It.Ref<bool?>.IsAny
                    )
                )
                .Returns(
                    (string id, out LocalizationTerm? term, out bool? isModded) =>
                    {
                        term = TermDictionary.ContainsKey(id) ? TermDictionary[id] : null;
                        isModded = false;
                        return term != null;
                    }
                );

            Container.RegisterInstance<IRegister<LocalizationTerm>>(termRegister.Object);

            // Initialize log storage
            LoggedMessages = new List<(LogLevel, object)>();

            // Mock IModLogger<T>
            var mockLogger = new Mock<IModLogger<CardDataPipeline>>();

            // Capture log messages in a list for assertions
            mockLogger
                .Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<object>()))
                .Callback<LogLevel, object>(
                    (level, data) =>
                    {
                        LoggedMessages.Add((level, data));
                    }
                );

            Container.RegisterInstance<IModLogger<CardDataPipeline>>(mockLogger.Object);

            Container.Register<CardDataPipeline>();
        }

        public static void Construct()
        {
            return;
        }

        public void Dispose() { }

        [Fact]
        public void Run_ShouldLoadCardsSuccessfully()
        {
            // Arrange
            var pipeline = Container.GetInstance<CardDataPipeline>();
            var mockCardRegister = new Mock<IRegister<CardData>>();

            // Act
            var results = pipeline.Run(mockCardRegister.Object);

            // Assert
            Assert.NotEmpty(results);
            var cardData = results[0];

            Assert.Equal("fire_starter", cardData.Id);
            Assert.Equal("Fire Starter", cardData.Data.name);

            // Check localization terms
            Assert.True(TermDictionary.ContainsKey("CardData_nameKey-fire_starter"));
            Assert.Equal("Fire Starter", TermDictionary["CardData_nameKey-fire_starter"].English);

            Assert.True(TermDictionary.ContainsKey("CardData_descriptionKey-fire_starter"));
            Assert.Equal(
                "Starts fires",
                TermDictionary["CardData_descriptionKey-fire_starter"].English
            );

            // Verify logger captured messages
            Assert.Contains(
                LoggedMessages,
                log =>
                    log.Level == LogLevel.Info
                    && log.Message.ToString()!.Contains("Overriding Card")
            );
        }

        [Fact]
        public void Run_ShouldHandleMissingIdGracefully()
        {
            // Arrange - Add invalid configuration
            var atlas = Container.GetInstance<PluginAtlas>();
            atlas.PluginDefinitions["test_plugin"].Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "cards:0:names:en", "Unnamed Card" }, // Missing "id"
                    }
                )
                .Build();

            var pipeline = Container.GetInstance<CardDataPipeline>();
            var mockCardRegister = new Mock<IRegister<CardData>>();

            // Act
            var results = pipeline.Run(mockCardRegister.Object);

            // Assert
            Assert.Empty(results);

            // Ensure no unexpected logs
            Assert.DoesNotContain(LoggedMessages, log => log.Level == LogLevel.Error);
        }

        [Fact]
        public void LoadCardConfiguration_ShouldRegisterNewCardCorrectly()
        {
            // Arrange
            var mockConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "id", "fire_starter" },
                        { "names:en", "Fire Starter" },
                        { "descriptions:en", "Starts fires" },
                        { "cost", "3" },
                    }
                )
                .Build();

            var pipeline = Container.GetInstance<CardDataPipeline>();
            var mockCardRegister = new Mock<IRegister<CardData>>();

            // Act
            var result = pipeline
                .GetType()
                .GetMethod(
                    "LoadCardConfiguration",
                    System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance
                )!
                .Invoke(
                    pipeline,
                    new object[] { mockCardRegister.Object, "test_plugin", mockConfig }
                );

            // Assert
            Assert.NotNull(result);
            var cardDataDefinition = (CardDataDefinition)result!;

            Assert.Equal("fire_starter", cardDataDefinition.Id);
            Assert.Equal(
                3,
                AccessTools.Field(typeof(CardData), "cost").GetValue(cardDataDefinition.Data)
            );

            // Verify localization term registration
            Assert.True(TermDictionary.ContainsKey("CardData_nameKey-fire_starter"));
            Assert.True(TermDictionary.ContainsKey("CardData_descriptionKey-fire_starter"));
        }

        [Fact]
        public void LoadCardConfiguration_ShouldLogWhenOverridingExistingCard()
        {
            // Arrange - Mock card lookup
            var existingCard = new CardData();

            var mockCardRegister = new Mock<IRegister<CardData>>();
            mockCardRegister
                .Setup(cr =>
                    cr.TryLookupName(
                        "fire_starter",
                        out It.Ref<CardData?>.IsAny,
                        out It.Ref<bool?>.IsAny
                    )
                )
                .Returns(
                    (string _, out CardData? card, out bool? modded) =>
                    {
                        modded = true;
                        card = existingCard;
                        return true;
                    }
                );

            var mockConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "id", "fire_starter" },
                        { "override", "true" },
                        { "names:english", "Fire Starter" },
                    }
                )
                .Build();

            var pipeline = Container.GetInstance<CardDataPipeline>();

            // Act
            pipeline
                .GetType()
                .GetMethod(
                    "LoadCardConfiguration",
                    System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance
                )!
                .Invoke(
                    pipeline,
                    new object[] { mockCardRegister.Object, "test_plugin", mockConfig }
                );

            // Assert
            Assert.Contains(
                LoggedMessages,
                log =>
                    log.Level == LogLevel.Info
                    && log.Message.ToString()!.Contains("Overriding Card fire_starter")
            );
        }
    }
}
