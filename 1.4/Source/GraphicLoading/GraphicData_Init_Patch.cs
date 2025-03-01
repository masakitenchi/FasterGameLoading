﻿using HarmonyLib;
using System.Collections.Generic;
using Verse;
using static FasterGameLoading.FasterGameLoadingMod;

namespace FasterGameLoading
{
    [HarmonyPatch(typeof(GraphicData), "Init")]
    public static class GraphicData_Init_Patch
    {
        public static Dictionary<string, List<GraphicData>> savedGraphics = new Dictionary<string, List<GraphicData>>();


        public static bool Prepare()
        {
            if(FishActive) return false;
            return true;
        }
        public static bool Prefix(GraphicData __instance)
        {
            if (__instance.texPath.NullOrEmpty() is false)
            {
                if (!savedGraphics.TryGetValue(__instance.texPath, out var graphicDatas))
                {
                    savedGraphics[__instance.texPath] = graphicDatas = new List<GraphicData>();
                }
                foreach (var item in graphicDatas)
                {
                    if (IsTheSameGraphicData(__instance, item) && item.cachedGraphic != null)
                    {
                        __instance.cachedGraphic = item.cachedGraphic;
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsTheSameGraphicData(GraphicData current, GraphicData other)
        {
            if (current.shaderParameters is null && other.shaderParameters is null
                && current.asymmetricLink is null && other.asymmetricLink is null)
            {
                if (current.color == other.color &&
                    current.colorTwo == other.colorTwo &&
                    current.graphicClass == other.graphicClass &&
                    current.drawSize == other.drawSize &&
                    current.linkType == other.linkType &&
                    current.linkFlags == other.linkFlags &&
                    current.shaderType == other.shaderType &&
                    current.drawOffset == other.drawOffset &&
                    current.drawOffsetEast == other.drawOffsetEast &&
                    current.drawOffsetNorth == other.drawOffsetNorth &&
                    current.drawOffsetSouth == other.drawOffsetSouth &&
                    current.drawOffsetWest == other.drawOffsetWest &&
                    current.allowAtlasing == other.allowAtlasing &&
                    current.allowFlip == other.allowFlip &&
                    current.drawRotated == other.drawRotated &&
                    current.renderInstanced == other.renderInstanced &&
                    current.flipExtraRotation == other.flipExtraRotation &&
                    current.onGroundRandomRotateAngle == other.onGroundRandomRotateAngle &&
                    current.overlayOpacity == other.overlayOpacity &&
                    current.renderQueue == other.renderQueue &&
                    current.maskPath == other.maskPath)
                {
                    //current.damageData == other.damageData &&
                    //current.shadowData == other.shadowData;
                    return true;
                }
            }
            return false;
        } 
    }
}
