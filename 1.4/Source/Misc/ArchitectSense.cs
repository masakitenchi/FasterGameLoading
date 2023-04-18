﻿using RimWorld;
using ArchitectSense;
using HarmonyLib;
using Verse;
using System;

namespace FasterGameLoading;

[HarmonyPatch]
public static class Patch_ArchitectSense
{
    private static Controller _instance;
    private static ModContentPack _contentPack;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Controller), MethodType.Constructor, new Type[] {typeof(ModContentPack)})]
    public static bool Prefix(ModContentPack content)
    {
        //_instance = __instance;
        _contentPack = content;
        return false;
    }

    public static void LoadAfterInit()
    {
        Log.Message("Initializing Architect Sense...");
        new Controller(_contentPack).Initialize();
    }
}
