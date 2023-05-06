using RimWorld;
using ArchitectSense;
using HarmonyLib;
using Verse;
using System;

namespace FasterGameLoading;

[HarmonyPatch]
public static class Patch_ArchitectSense
{
    private static ModContentPack _contentPack;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Controller), MethodType.Constructor, new Type[] { typeof(ModContentPack) })]
    public static bool Prefix(ModContentPack content)
    {
        //_instance = __instance;
        if (FasterGameLoadingSettings.delayArchitectSenseLoading)
        {
            _contentPack = content;
            return false;
        }
        return true;
    }

    public static void LoadAfterInit()
    {
        new Controller(_contentPack).Initialize();
    }
}