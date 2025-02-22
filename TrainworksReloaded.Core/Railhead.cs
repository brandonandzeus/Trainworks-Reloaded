using Microsoft.Extensions.Configuration;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Core
{
    public class Railhead
    {
        private static LazyBuilder lazyConfigure = new LazyBuilder();

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
