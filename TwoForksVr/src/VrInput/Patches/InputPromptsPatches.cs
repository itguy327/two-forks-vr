using HarmonyLib;
using TMPro;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.VrInput.Patches
{
    [HarmonyPatch]
    public class InputPromptsPatches : TwoForksVrPatch
    {
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.GetIconName))]
        // private static bool ReplacePromptIconsWithVrButtonText(ref string __result, string id)
        // {
        //     var inputAction = StageInstance.GetInputAction(id);
        //     if (inputAction == null)
        //     {
        //         Logs.LogWarning($"Failed to find actionInput for virtual key {id}");
        //         return true;
        //     }
        //
        //     __result = inputAction.Action.GetLocalizedOriginPart(SteamVR_Input_Sources.Any,
        //         EVRInputStringBits.VRInputString_InputSource);
        //
        //     return false;
        // }
        //
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.HasIcon))]
        // private static bool ReplacePromptIconsWithVrButtonText(ref bool __result)
        // {
        //     __result = true;
        //     return false;
        // }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InlineGraphicManager), nameof(InlineGraphicManager.LoadSpriteAsset))]
        private static void TestGraphic(InlineGraphicManager __instance, SpriteAsset spriteAsset)
        {
            var texture = VrAssetLoader.faceButtonSpriteSheet;
            const float textureSize = 1024f;
            const int rowCount = 10;
            const float iconSize = 100;

            spriteAsset.spriteSheet = VrAssetLoader.faceButtonSpriteSheet;

            foreach (var info in spriteAsset.spriteInfoList)
            {
                var x = info.id / rowCount;
                var y = info.id % rowCount;
                Logs.LogInfo($"id {info.id}: x={x}, y={y}");
                info.x = x * iconSize;
                info.y = y * iconSize;
                info.width = info.height = iconSize;
                info.pivot = Vector2.zero;
                info.scale = 3;
                info.xOffset = info.yOffset = 0;
                info.xAdvance = iconSize;
            }

            // ID for click should be 38.
        }
    }
}