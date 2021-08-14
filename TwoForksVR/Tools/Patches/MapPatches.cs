﻿using System;
using HarmonyLib;
using UnityEngine;

namespace TwoForksVR.Tools.Patches
{
    [HarmonyPatch]
    public static class MapPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AmplifyMotionObjectBase), "Start")]
        private static void CreateVRMap(AmplifyMotionObjectBase __instance)
        {
            // The MapInHand object doesn't have any specific component that I can attach to.
            // So I'm using AmplifyMotionObject, which is present in a bunch of objects in the game,
            // then filtering by the name.
            if (__instance.name != "MapInHand") return;
            VRMap.Create(__instance.transform, "Left");
        }
    }
}