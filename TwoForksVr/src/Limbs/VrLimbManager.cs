using TwoForksVr.Climbing;
using TwoForksVr.LaserPointer;
using TwoForksVr.Stage;
using TwoForksVr.Tools;
using TwoForksVr.UI.Patches;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Limbs
{
    public class VrLimbManager : MonoBehaviour
    {
        private Laser laser;
        private VrHand leftHand;
        private VrHand rightHand;
        private VrClimbing leftClimbingController;
        private VrClimbing rightClimbingController;

        public static VrLimbManager Create(VrStage stage)
        {
            var instance = new GameObject("VrLimbManager").AddComponent<VrLimbManager>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(stage.transform, false);
            
            instance.rightHand = VrHand.Create(instanceTransform);
            instance.rightClimbingController = VrClimbing.Create(instance.rightHand);

            instance.leftHand = VrHand.Create(instanceTransform, true);
            instance.leftClimbingController = VrClimbing.Create(instance.leftHand);
            
            ToolPicker.Create(
                instanceTransform,
                instance.leftHand.transform,
                instance.rightHand.transform
            );
            instance.laser = Laser.Create(
                instance.leftHand.transform,
                instance.rightHand.transform
            );

            InventoryPatches.RightHand = instance.rightHand.transform;

            return instance;
        }

        public void SetUp(Transform playerTransform, Camera camera)
        {
            var skeletonRoot = GetSkeletonRoot(playerTransform);
            var armsMaterial = GetArmsMaterial(playerTransform);
            rightHand.SetUp(skeletonRoot, armsMaterial);
            leftHand.SetUp(skeletonRoot, armsMaterial);
            laser.SetUp(camera);
            
            rightClimbingController.SetUp(playerTransform, leftClimbingController);
            leftClimbingController.SetUp(playerTransform, rightClimbingController);

            VrFoot.Create(skeletonRoot);
            VrFoot.Create(skeletonRoot, true);
        }

        public void HighlightButton(params ISteamVR_Action_In_Source[] actions)
        {
            if (actions.Length == 0)
            {
                leftHand.ButtonHighlight.HideAllButtonHints();
                rightHand.ButtonHighlight.HideAllButtonHints();
            }
            else
            {
                leftHand.ButtonHighlight.ShowButtonHint(actions);
                rightHand.ButtonHighlight.ShowButtonHint(actions);
            }
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