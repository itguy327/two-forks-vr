﻿using System;
using HarmonyLib;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class MenuCameraPatches
    {
        // Not sure how to get the PlayMaker reference to work in this project, so have to use reflection instead. 
        private static readonly Type playMakerFsmType = Type.GetType("PlayMakerFSM, PlayMaker");

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMenuCameraController), nameof(vgMenuCameraController.Start))]
        private static void CreateMenuStage(vgMenuCameraController __instance)
        {
            VrStage.Instance.SetUp(__instance.GetComponentInChildren<Camera>(), null);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMenuCameraController), nameof(vgMenuCameraController.Start))]
        private static void DisableMainMenuCameraAnimation(vgMenuCameraController __instance)
        {
            var playMakerFsm = __instance.gameObject.GetComponentInParent(playMakerFsmType);
            playMakerFsmType.GetProperty("enabled")?.SetValue(playMakerFsm, false, new object[] { });
        }
    }
}