using Castle.Core.Configuration;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Moq;
using SimpleInjector;
using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using TrainworksReloaded.Plugin;
using UnityEngine;

namespace TrainworksReloaded.Test
{
    public class CardEffectTests : IDisposable
    {
        public Container Container { get; set; }
        public List<(LogLevel Level, object Message)> LoggedMessages { get; set; }

        class CardEffectDamage : global::CardEffectDamage { }
        class CardEffectInvalidCustom { }

        public CardEffectTests()
        {
            Container = new Container();

            var atlas = new PluginAtlas();
            Container.RegisterInstance<PluginAtlas>(atlas);

            //Instance Generator
            Container.RegisterConditional(
                typeof(IInstanceGenerator<>),
                typeof(InstanceGenerator<>),
                c => !c.Handled
            );

            // Initialize log storage
            LoggedMessages = [];

            // Mock IModLogger<T>
            var mockLogger = new Mock<IModLogger<CardEffectDataPipeline>>();

            // Capture log messages in a list for assertions
            mockLogger
                .Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<object>()))
                .Callback<LogLevel, object>(
                    (level, data) =>
                    {
                        LoggedMessages.Add((level, data));
                    }
                );

            Container.RegisterInstance<IModLogger<CardEffectDataPipeline>>(mockLogger.Object);

            Container.Register<CardEffectDataPipeline>();

            Type? returnedTypeDamage = typeof(global::CardEffectDamage);
            Type? returnedTypeStatus = typeof(CardEffectAddStatusEffect);
            Type? returnedTypeCustomDamage = typeof(CardEffectDamage);
            Type? returnedTypeInvalidCustom = typeof(CardEffectInvalidCustom);
            Type? returnedNull;

            var mockGame = new Mock<ITypeProvider>();
            mockGame.Setup(xs => xs.TryLookupType("CardEffectWinGame", out returnedNull)).Returns(false);
            mockGame.Setup(xs => xs.TryLookupType("CardEffectInvalidCustom", out returnedNull)).Returns(false);
            mockGame.Setup(xs => xs.TryLookupType("CardEffectDamage", out returnedTypeDamage)).Returns(true);

            var mockLib1 = new Mock<ITypeProvider>();
            mockLib1.Setup(xs => xs.TryLookupType("CardEffectWinGame", out returnedNull)).Returns(false);
            mockLib1.Setup(xs => xs.TryLookupType("CardEffectInvalidCustom", out returnedNull)).Returns(false);
            mockLib1.Setup(xs => xs.TryLookupType("CardEffectDamage", out returnedTypeCustomDamage)).Returns(true);

            var mockLib2 = new Mock<ITypeProvider>();
            mockLib2.Setup(xs => xs.TryLookupType("CardEffectWinGame", out returnedNull)).Returns(false);
            mockLib2.Setup(xs => xs.TryLookupType("CardEffectInvalidCustom", out returnedTypeInvalidCustom)).Returns(true);
            mockLib2.Setup(xs => xs.TryLookupType("CardEffectDamage", out returnedNull)).Returns(false);

            var assemblyMocks = new List<Mock<ITypeProvider>>()
            {
                mockLib1,
                mockLib2
            };
            var mockAssemblies = new Dictionary<string, ITypeProvider>()
            {
                ["com.mymodhere"] = assemblyMocks[0].Object,
                ["com.modlibrary2"] = assemblyMocks[1].Object,
            };

            Container.RegisterInstance<ITypeResolver>(new TypeResolver(mockGame.Object, mockAssemblies));
        }

        public void Dispose() { }

        [Fact]
        public void LoadCardConfiguration_ShouldRegisterNewEffectCorrectly()
        {
            new CardEffectData();
            // Arrange
            var mockConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "id", "DamagingCardEffectVanilla" },
                        { "name", "CardEffectDamage" },
                    }
                )
                .Build();

            var pipeline = Container.GetInstance<CardEffectDataPipeline>();
            var mockCardRegister = new Mock<IRegister<CardEffectData>>();

            // Act
            var result = pipeline.LoadEffectConfiguration(
                mockCardRegister.Object,
                "com.modlibrary2",
                mockConfig
            );

            // Assert
            Assert.NotNull(result);
            var cardEffectDataDefinition = (CardEffectDefinition)result!;

            Assert.Equal("DamagingCardEffectVanilla", cardEffectDataDefinition.Id);
            Assert.Equal(
                "CardEffectDamage",
                AccessTools.Field(typeof(CardData), "effectStateName").GetValue(cardEffectDataDefinition.Data)
            );
        }
    }
}
