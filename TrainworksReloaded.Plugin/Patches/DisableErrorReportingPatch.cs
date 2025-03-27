﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainworksReloaded.Plugin.Patches
{
    // Prevent sending error reports to ShinyShoe.
    [HarmonyPatch(typeof(ShinyShoe.AppManager), "DoesThisBuildReportErrors")]
    class DisableErrorReportingPatch
    {
        public static void Postfix(ref bool __result)
        {
            __result = false;
        }
    }
}
