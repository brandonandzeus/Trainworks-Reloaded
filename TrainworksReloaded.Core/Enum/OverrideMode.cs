using System;
using System.Collections.Generic;
using System.Text;

namespace TrainworksReloaded.Core.Enum
{
    public enum OverrideMode
    {
        New,
        Replace,
        Append,
        Clone
    }

    public static class OverrideModeExtensions
    {
        public static bool IsNewContent(this OverrideMode overrideMode)
        {
            return overrideMode == OverrideMode.New;
        }

        public static bool IsOverriding(this OverrideMode overrideMode)
        {
            return overrideMode == OverrideMode.Replace || overrideMode == OverrideMode.Append;
        }

        public static bool IsCloning(this OverrideMode overrideMode)
        {
            return overrideMode == OverrideMode.Clone;
        }
    }
}
