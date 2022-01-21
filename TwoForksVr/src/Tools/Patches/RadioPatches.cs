using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Tools.Patches
{
    [HarmonyPatch]
    public static class RadioPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgAttachPoint), nameof(vgAttachPoint.Attach))]
        private static void SwapHandOnAttach(vgAttachPoint __instance)
        {
            Logs.LogWarning("#### PickRadioAttachHand");
            var grip = SteamVR_Actions._default.Grip;
            var isGrippingRight = grip.GetState(SteamVR_Input_Sources.RightHand);
            var isGrippingLeft = grip.GetState(SteamVR_Input_Sources.LeftHand);
            var previousAttachTransform = __instance.attachTransform;
            
            if (isGrippingRight)
            {
                Logs.LogWarning("#### isGrippingRight");
                __instance.attachTransform = "henryHandRightAttachment";
            } else if (isGrippingLeft)
            {
                Logs.LogWarning("#### isGrippingLeft");
                __instance.attachTransform = "henryHandLeftAttachment";
            }

            if (previousAttachTransform != __instance.attachTransform)
            {
                __instance.attachRotation.Scale(new Vector3(1, 1, -1));
            }
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerRadioControl), nameof(vgPlayerRadioControl.OnEquipRadio))]
        private static void AttachOnEquip(vgPlayerRadioControl __instance)
        {
            var isAttachSuccess = __instance.radioHandAttach.Attach(__instance.transform, __instance.radio.transform);
            Logs.LogWarning($"#### isAttachSuccess {isAttachSuccess}");
        }
    }
}