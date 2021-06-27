﻿using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Raicuparta.TwoForksVR
{
    public class VRMap: MonoBehaviour
    {
        private void Start()
        {
            // Very hard to read the map in VR since we can't zoom in.
            // So making it bigger to make it easier, especially for lower resolution headsets.
            transform.localScale = Vector3.one * 150f;

            // Need to disable and re-enable the cloth component to reset it,
            // otherwise the physics would be all fucky after resizing.
            var cloth = gameObject.GetComponent<Cloth>();
            cloth.enabled = false;
            cloth.enabled = true;
        }

        // In base game, map resolution changes depending on zoom. In VR there's no zoom,
        // so we need to always have high resolution. For whatever reason I don't seem to be
        // able to patch the IsZoomingMap method, so I'm forcing it on LateUpdate instead.
        private void ForceHighResolutionMap()
        {
            var mapController = vgMapManager.Instance?.mapController;
            if (!mapController) return;

            mapController.isZoomingMap = true;
        }

        private void LateUpdate()
        {
            ForceHighResolutionMap();
        }

        [HarmonyPatch(typeof(vgMapController), "OnEquipMap")]
        public class PatchEquipMap
        {
            private static bool originalFlag = false;

            public static void Prefix(ref bool ___compassEquipped)
            {       
                originalFlag = ___compassEquipped;
                ___compassEquipped = true;
            }

            public static void Postfix(ref bool ___compassEquipped)
            {
                ___compassEquipped = originalFlag;
            }
        }

        [HarmonyPatch(typeof(vgMapController), "OnUnequipMap")]
        public class patchUnequipMap
        {
            public static bool Prefix(vgMapController __instance, float axis)
            {
                __instance.UnEquipMap(axis);
                return false;
            }
        }
    }
}
