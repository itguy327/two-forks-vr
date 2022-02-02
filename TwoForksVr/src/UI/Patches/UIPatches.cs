﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class UIPatches : TwoForksVrPatch
    {
        private static readonly Dictionary<string, Material> materialMap = new Dictionary<string, Material>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.ShowAbilityIcon))]
        private static bool PreventShowingAbilityIcon()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.InitializeAbilityIcon))]
        private static bool DestroyAbilityIcon(vgHudManager __instance)
        {
            Object.Destroy(__instance.abilityIcon);
            return false;
        }

        // For some reason, the default text shader draws on top of everything.
        // I'm importing the TMPro shader from a more recent version and replacing it in the font materials.
        // This way, I can decide which ones I actually want to draw on top.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.Awake))]
        [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.OnEnable))]
        private static void PreventTextFromDrawingOnTop(TextMeshProUGUI __instance)
        {
            try
            {
                if (__instance.canvas && !__instance.canvas.GetComponent<GraphicRaycaster>()) return;

                var key = __instance.font.name;

                if (!materialMap.ContainsKey(key))
                    materialMap[key] = new Material(__instance.font.material)
                    {
                        shader = VrAssetLoader.TMProShader
                    };

                __instance.fontMaterial = materialMap[key];
                __instance.fontBaseMaterial = materialMap[key];
                __instance.fontSharedMaterial = materialMap[key];
            }
            catch (Exception exception)
            {
                Logs.LogWarning($"Error in TMPro Patch ({__instance.name}): {exception}");
            }
        }
    }
}