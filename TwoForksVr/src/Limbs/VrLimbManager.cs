﻿using TwoForksVr.LaserPointer;
using TwoForksVr.Stage;
using TwoForksVr.Tools;
using TwoForksVr.UI.Patches;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public class VrLimbManager : MonoBehaviour
    {
        public VrLaser Laser;
        private ToolPicker toolPicker;
        public VrHand LeftHand { get; private set; }
        public VrHand RightHand { get; private set; }
        public bool IsToolPickerOpen => toolPicker && toolPicker.IsOpen;

        public static VrLimbManager Create(VrStage stage)
        {
            var instance = new GameObject("VrLimbManager").AddComponent<VrLimbManager>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(stage.transform, false);

            instance.RightHand = VrHand.Create(instanceTransform);
            instance.LeftHand = VrHand.Create(instanceTransform, true);
            instance.toolPicker = ToolPicker.Create(
                instanceTransform,
                instance.LeftHand.transform,
                instance.RightHand.transform
            );
            instance.Laser = VrLaser.Create(
                instance.LeftHand.transform,
                instance.RightHand.transform
            );

            InventoryPatches.RightHand = instance.RightHand.transform;

            return instance;
        }

        public void SetUp(vgPlayerController playerController, Camera camera)
        {
            var playerTransform = playerController ? playerController.transform : null;
            var skeletonRoot = GetSkeletonRoot(playerTransform);
            var armsMaterial = GetArmsMaterial(playerTransform);
            RightHand.SetUp(skeletonRoot, armsMaterial, playerController);
            LeftHand.SetUp(skeletonRoot, armsMaterial, playerController);
            Laser.SetUp(camera);
        }

        private static Material GetArmsMaterial(Transform playerTransform)
        {
            return !playerTransform
                ? null
                : playerTransform.Find("henry/body")?.GetComponent<SkinnedMeshRenderer>().materials[2];
        }

        private static Transform GetSkeletonRoot(Transform playerTransform)
        {
            return playerTransform ? playerTransform.Find("henry/henryroot") : null;
        }
    }
}