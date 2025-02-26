using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Core
{
    public class Railhead
    {
        private static readonly LazyBuilder lazyConfigure = new();

        public static ILazyBuilder GetBuilder()
        {
            return lazyConfigure;
        }

        internal static LazyBuilder GetBuilderForInit()
        {
            return lazyConfigure;
        }
    }
}
