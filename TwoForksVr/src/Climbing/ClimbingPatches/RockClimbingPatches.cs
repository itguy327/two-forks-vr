using System.Linq;
using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Climbing.ClimbingPatches
{
    [HarmonyPatch]
    public static class RockClimbingPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgBaseMoveEdge), nameof(vgBaseMoveEdge.Start))]
        public static void SetUpRockClimbing(vgBaseMoveEdge __instance)
        {
            Logs.LogInfo($"#### Found vgBaseMoveEdge");
            // var colliders = __instance.transform.parent.GetComponentsInChildren<Collider>();
            // var firstActiveCollider = colliders.First(collider => collider.enabled);
            // Logs.LogInfo($"#### Found collider {firstActiveCollider.name} {firstActiveCollider.transform.GetSiblingIndex().ToString()}");
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.Start))]
        public static void SetUpCharacterController(vgPlayerController __instance)
        {
            __instance.characterController.radius = 0.2f;
        }
    }
}