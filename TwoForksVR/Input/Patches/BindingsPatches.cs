﻿using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Valve.VR;

namespace TwoForksVR.Input.Patches
{
    [HarmonyPatch]
    public static class BindingsPatches
    {
        private static SteamVR_Input_ActionSet_default actionSet;
        private static Dictionary<string, SteamVR_Action_Boolean> booleanActionMap;
        private static Dictionary<string, SteamVR_Action_Vector2> vector2XActionMap;
        private static Dictionary<string, SteamVR_Action_Vector2> vector2YActionMap;

        private static void Initialize()
        {
            actionSet = SteamVR_Actions._default;
            booleanActionMap = new Dictionary<string, SteamVR_Action_Boolean>
            {
                {InputName.LocomotionAction, actionSet.Interact},
                {InputName.Use, actionSet.Interact},
                {InputName.UISubmit, actionSet.Interact},
                {InputName.StowHeldObject, actionSet.Jog},
                {InputName.UICancel, actionSet.Cancel},
                {InputName.DialogSelectionUp, actionSet.UIUp},
                {InputName.DialogSelectionDown, actionSet.UIDown},
                {InputName.UIUp, actionSet.UIUp},
                {InputName.UIDown, actionSet.UIDown},
                {InputName.LockNumberUp, actionSet.UIUp},
                {InputName.LockNumberDown, actionSet.UIDown},
                {InputName.LockTumblerRight, actionSet.NextPage},
                {InputName.LockTumblerLeft, actionSet.PreviousPage},
                {InputName.LockCancel, actionSet.Cancel},
                {InputName.ToggleJog, actionSet.Jog},
                {InputName.Pause, actionSet.Cancel},
                {InputName.NextMenu, actionSet.NextPage},
                {InputName.PreviousMenu, actionSet.PreviousPage}
            };
            vector2XActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveStrafe, actionSet.Move},
                {InputName.LookHorizontalStick, actionSet.Rotate},
                {InputName.UIHorizontal, actionSet.Move},
            };
            vector2YActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveForward, actionSet.Move},
                {InputName.LookVerticalStick, actionSet.Rotate},
                {InputName.Scroll, actionSet.Move},
                {InputName.UIVertical, actionSet.Move},
            };

            // Pick dialog option with interact button.
            // TODO: move this somewhere else.
            actionSet.Interact.onStateDown += (fromAction, fromSource) =>
            {
                if (!vgDialogTreeManager.Instance) return;
                vgDialogTreeManager.Instance.OnConfirmDialogChoice();
                vgDialogTreeManager.Instance.ClearNonRadioDialogChoices();
            };

            foreach (var entry in vector2XActionMap)
            {
                entry.Value.onChange += (action, source, axis, delta) => TriggerCommand(entry.Key, axis.x);
            }
            foreach (var entry in vector2YActionMap)
            {
                entry.Value.onChange += (action, source, axis, delta) => TriggerCommand(entry.Key, axis.y);
            }
            foreach (var entry in booleanActionMap)
            {
                entry.Value.onChange += (action, source, state) =>
                {
                    if (!state) return;
                    TriggerCommand(entry.Key, 1);
                };
            }
        }
        
        public static void TriggerCommand(string command, float axisValue)
        {
            if (!vgInputManager.Instance || vgInputManager.Instance.commandCallbackMap == null) return;
		    var commandCallbackMap = vgInputManager.Instance.commandCallbackMap;
            if (!vgInputManager.Instance.flushCommands && commandCallbackMap.TryGetValue(command, out var inputDelegate))
            {
                inputDelegate?.Invoke(axisValue);
            }
	    }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgAxisData), nameof(vgAxisData.Update))]
        private static void ReadAxisValuesFromSteamVR(vgAxisData __instance)
        {
            if (!SteamVR_Input.initialized) return;
            
            // TODO move this elsewhere.
            if (actionSet == null) Initialize();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamVR_Input), nameof(SteamVR_Input.GetActionsFileFolder))]
        private static bool GetActionsFileFromMod(ref string __result)
        {
            // TODO: could probably just use the streamingassets folder and avoid doing this?
            __result = $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/TwoForksVR/Bindings";
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgInputManager), nameof(vgInputManager.Awake))]
        [HarmonyPatch(typeof(vgInputManager), nameof(vgInputManager.SetControllerLayout))]
        [HarmonyPatch(typeof(vgInputManager), nameof(vgInputManager.SetKeyBindFromPlayerPrefs))]
        private static void ForceXboxController(vgInputManager __instance)
        {
            __instance.currentControllerLayout = vgControllerLayoutChoice.XBox;
        }
    }
}