using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Test
{
    public class TypeResolverTest
    {
        public TypeResolver typeResolver;
        public Type? returnedTypeDamage;
        public Type? returnedTypeStatus;
        public Type? returnedTypeCustomDamage;
        public Type? returnedTypeCustom2;
        public Type? returnedNull;

        class CardEffectDamage : global::CardEffectDamage {}
        class CardEffectCustom { }

        public TypeResolverTest()
        {
            returnedTypeDamage = typeof(global::CardEffectDamage);
            returnedTypeStatus = typeof(CardEffectAddStatusEffect);
            returnedTypeCustomDamage = typeof(CardEffectDamage);
            returnedTypeCustom2 = typeof(CardEffectCustom);

            var mockGame = new Mock<ITypeProvider>();
            mockGame.Setup(xs => xs.TryLookupType("CardEffectWinGame", out returnedNull)).Returns(false);
            mockGame.Setup(xs => xs.TryLookupType("CardEffectCustom", out returnedNull)).Returns(false);
            mockGame.Setup(xs => xs.TryLookupType("CardEffectDamage", out returnedTypeDamage)).Returns(true);

            var mockLib1 = new Mock<ITypeProvider>();
            mockLib1.Setup(xs => xs.TryLookupType("CardEffectWinGame", out returnedNull)).Returns(false);
            mockLib1.Setup(xs => xs.TryLookupType("CardEffectCustom", out returnedNull)).Returns(false);
            mockLib1.Setup(xs => xs.TryLookupType("CardEffectDamage", out returnedTypeCustomDamage)).Returns(true);
            
            var mockLib2 = new Mock<ITypeProvider>();
            mockLib2.Setup(xs => xs.TryLookupType("CardEffectWinGame", out returnedNull)).Returns(false);
            mockLib2.Setup(xs => xs.TryLookupType("CardEffectCustom", out returnedTypeCustom2)).Returns(true);
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
            typeResolver = new TypeResolver(mockGame.Object, mockAssemblies);
        }

        [Fact]
        public void Run_ShouldFindBaseGameDamage()
        {
            Assert.True(typeResolver.TryResolveType("CardEffectDamage", "com.modlibrary2", out Type? returned, out bool? baseGame));
            Assert.True(baseGame);
            Assert.Equal(returned, returnedTypeDamage);
        }

        [Fact]
        public void Run_ShouldFindOverrideDamage()
        {
            Assert.True(typeResolver.TryResolveType("CardEffectDamage", "com.mymodhere", out Type? returned, out bool? baseGame));
            Assert.False(baseGame);
            Assert.Equal(returned, returnedTypeCustomDamage);
        }

        [Fact]
        public void Run_ShouldFindCustom()
        {
            Assert.True(typeResolver.TryResolveType("CardEffectCustom", "com.modlibrary2", out Type? returned, out bool? baseGame));
            Assert.False(baseGame);
            Assert.Equal(returned, returnedTypeCustom2);
        }

        [Fact]
        public void Run_ShouldNotFindCustom()
        {
            Assert.False(typeResolver.TryResolveType("CardEffectCustom", "com.mymodhere", out Type? returned, out bool? baseGame));
            Assert.False(baseGame);
            Assert.Equal(returned, returnedNull);
        }

        [Fact]
        public void Run_ShouldNotFindNonexistingType()
        {
            Assert.False(typeResolver.TryResolveType("CardEffectWinGame", "com.mymodhere", out Type? returned, out bool? baseGame));
            Assert.False(baseGame);
            Assert.Equal(returned, returnedNull);

            Assert.False(typeResolver.TryResolveType("CardEffectWinGame", "com.modlibrary2", out returned, out baseGame));
            Assert.False(baseGame);
            Assert.Equal(returned, returnedNull);
        }

        [Fact]
        public void Run_WithNonexistingModGUID()
        {
            Assert.True(typeResolver.TryResolveType("CardEffectDamage", "com.nonexisting", out Type? returned, out bool? baseGame));
            Assert.True(baseGame);
            Assert.Equal(returned, returnedTypeDamage);

            Assert.False(typeResolver.TryResolveType("CardEffectWinGame", "com.nonexisting", out returned, out baseGame));
            Assert.False(baseGame);
            Assert.Equal(returned, returnedNull);
        }
    }
}
