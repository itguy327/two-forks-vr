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
        [HarmonyPatch(typeof(vgRockClimbEdge), nameof(vgRockClimbEdge.InitializeEdgeType))]
        public static void SetUpRockClimbing(vgRockClimbEdge __instance)
        {
            // TODO is edge needed? or do this some other way?
            __instance.enabled = false;
            __instance.edge = null;
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