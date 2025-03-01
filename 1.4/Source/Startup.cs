﻿using HarmonyLib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace FasterGameLoading
{
    [HarmonyPatch]
    public static class Startup
    {
        public static readonly string[] AllExceptionMods = {
            "CombatAI",
            "CombatExtended.ExtendedLoadout",
            "LunarLoader",
            "LunarFramework",
            "MapPreview",
            "MapPreviewMod",
            "GeologicalLandforms",
            "GeologicalLandformsMod",
            "TerrainGraph",
            "PrepareLanding",
            "ArchitectSense",
            "ImprovedWorkbenches"
        };

        //public static StringBuilder sb = new StringBuilder("Delaying these LongEvent:\n");

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return ModsConfig.ActiveModsInLoadOrder.Any(x => x.Name == "BetterLoading")
                ? AccessTools.Method("BetterLoading.BetterLoadingMain:CreateTimingReport")
                : (MethodBase)AccessTools.Method(typeof(StaticConstructorOnStartupUtility), "CallAll");
        }

        public static void Prefix()
        {
            if (FasterGameLoadingSettings.delayHarmonyPatchesLoading)
            {
                FasterGameLoadingMod.harmony.Patch(AccessTools.DeclaredMethod(typeof(Harmony), nameof(Harmony.PatchAll),
                    new Type[] { typeof(Assembly) }), prefix: new HarmonyMethod(AccessTools.Method(typeof(Startup), nameof(DelayHarmonyPatchAll))));
                doNotDelayHarmonyPatches = false;
            }
        }

        public static bool doNotDelayHarmonyPatches = true;
        public static bool DelayHarmonyPatchAll(Harmony __instance, Assembly assembly)
        {
            if (doNotDelayHarmonyPatches || AllExceptionMods.Contains(assembly.GetName().Name)) return true;
            //Log.Message($"Delaying {assembly.FullName}'s harmony patches");
            FasterGameLoadingMod.delayedActions.harmonyPatchesToPerform.Add((__instance, assembly));
            return false;
        }

        public static bool doNotDelayLongEventsWhenFinished = true;
        public static bool DelayExecuteWhenFinished(Action action)
        {
            if (doNotDelayLongEventsWhenFinished) return true;
            if (action.Method.Name.Contains("DoPlayLoad") is false && action.Method.DeclaringType.Assembly == typeof(Game).Assembly)
            {
                //sb.AppendLine($"{action.Method.DeclaringType} {action.Method.Name}");
                FasterGameLoadingMod.delayedActions.actionsToPerform.Add(action);
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void Postfix()
        {
            FasterGameLoadingSettings.modsInLastSession = ModsConfig.ActiveModsInLoadOrder.Select(x => x.packageIdLowerCase).ToList();
            FasterGameLoadingSettings.loadedTexturesSinceLastSession = ModContentLoaderTexture2D_LoadTexture_Patch.loadedTexturesThisSession;
            FasterGameLoadingSettings.loadedTypesByFullNameSinceLastSession = GenTypes_GetTypeInAnyAssemblyInt_Patch.loadedTypesThisSession;
            FasterGameLoadingSettings.successfulXMLPathesSinceLastSession = XmlNode_SelectSingleNode_Patch.successfulXMLPathesThisSession;
            FasterGameLoadingSettings.failedXMLPathesSinceLastSession = XmlNode_SelectSingleNode_Patch.failedXMLPathesThisSession;
            LoadedModManager.GetMod<FasterGameLoadingMod>().WriteSettings();
            LongEventHandler.toExecuteWhenFinished.Add(delegate
            {
                FasterGameLoadingMod.delayedActions.StartCoroutine(FasterGameLoadingMod.delayedActions.PerformActions());
            });
            //Log.Message(sb.ToString());
        }
    }
}

