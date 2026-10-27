using System;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using Verse;

namespace DubsLiteAsyncSewageFix
{
    /// <summary>
    /// Narrow mitigation for background map generation invoking DBH Lite's
    /// sewage map mesh layer, which should not be needed in Lite mode.
    /// Does not modify the original DBH DLL or any game saves.
    /// </summary>
    public sealed class DubsLiteAsyncSewageFixMod : Mod
    {
        private static int mainThreadId;

        public DubsLiteAsyncSewageFixMod(ModContentPack content) : base(content)
        {
            // Mod constructors normally run during mod initialization on the Unity thread.
            mainThreadId = Thread.CurrentThread.ManagedThreadId;

            Type targetType = AccessTools.TypeByName("DubsBadHygiene.XSectionLayer_Sewage");
            MethodInfo targetMethod = targetType == null
                ? null
                : AccessTools.Method(targetType, "Regenerate");

            if (targetMethod == null)
            {
                Log.Warning("[Dubs Lite Async Sewage Fix] Target not found; mod unchanged. " +
                            "Check that Dubs Bad Hygiene Lite 1.6 is installed.");
                return;
            }

            var harmony = new Harmony("local.dubslite.asyncsewagefix");
            MethodInfo prefix = AccessTools.Method(typeof(DubsLiteAsyncSewageFixMod),
                                                   nameof(AllowSewageLayerOnMainThread));
            harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefix));
            Log.Message("[Dubs Lite Async Sewage Fix] Active: sewage layer mesh " +
                        "regeneration will be skipped on non-main threads.");
        }

        public static bool AllowSewageLayerOnMainThread()
        {
            // true -> run original mesh-building code; false -> skip it.
            return Thread.CurrentThread.ManagedThreadId == mainThreadId;
        }
    }
}
