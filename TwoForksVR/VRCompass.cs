﻿using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    public class VRCompass
    {
        [HarmonyPatch(typeof(vgCompass), "LateUpdate")]
        public class PatchCompass
        {
            public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
            {
                float unsignedAngle = Vector3.Angle(from, to);

                float cross_x = from.y * to.z - from.z * to.y;
                float cross_y = from.z * to.x - from.x * to.z;
                float cross_z = from.x * to.y - from.y * to.x;
                float sign = Mathf.Sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
                return unsignedAngle * sign;
            }

            public static bool Prefix(vgCompass __instance, Vector3 ___newRotation, float ___worldOffset)
            {
                var transform = __instance.transform;
                var forward = Vector3.ProjectOnPlane(-transform.parent.forward, Vector3.up);
                var angle = SignedAngle(forward, Vector3.forward, Vector3.up);
                ___newRotation.y = angle - 165f - ___worldOffset;
                transform.localEulerAngles = ___newRotation;
                return false;
            }
        }

        [HarmonyPatch(typeof(vgCompass), "Start")]
        public class PatchCompassStart
        {
            public static void Postfix(vgCompass __instance)
            {
                __instance.transform.parent.gameObject.AddComponent<DebugAxes>();
            }
        }
    }
}
