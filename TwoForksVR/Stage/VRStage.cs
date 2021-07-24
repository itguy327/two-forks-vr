﻿using MelonLoader;
using TwoForksVR.Hands;
using TwoForksVR.PlayerCamera;
using UnityEngine;

namespace TwoForksVR.Stage
{
    public class VRStage: MonoBehaviour
    {
        public static VRStage Instance;

        private VRCameraManager cameraManager;
        private VRHandsManager handsManager;
        private LateUpdateFollow follow;
        private Camera mainCamera;
        private Camera fallbackCamera;
        private IntroFix introFix;

        public static VRStage Create()
        {
            if (!Instance)
            {
                var stageParent = new GameObject("VRStageParent");

                // Apparently Firewatch will destroy all DontDrestroyOnLoad objects between scenes,
                // unless they have the MAIN tag.
                stageParent.tag = "MAIN";

                DontDestroyOnLoad(stageParent);
                Instance = new GameObject("VRStage").AddComponent<VRStage>();
                Instance.transform.SetParent(stageParent.transform, false);
                Instance.cameraManager = VRCameraManager.Create(Instance);
                Instance.handsManager = VRHandsManager.Create(Instance);
                Instance.follow = stageParent.AddComponent<LateUpdateFollow>();

                Instance.fallbackCamera = new GameObject("VRFallbackCamera").AddComponent<Camera>();
                Instance.fallbackCamera.enabled = false;
                Instance.fallbackCamera.clearFlags = CameraClearFlags.Color;
                Instance.fallbackCamera.backgroundColor = Color.black;
                Instance.fallbackCamera.tag = "MainCamera";
                Instance.fallbackCamera.transform.SetParent(Instance.transform, false);
            }
            return Instance;
        }

        public void SetUp(Camera camera, Transform playerTransform)
        {
            mainCamera = camera;
            follow.Target = mainCamera?.transform.parent;
            if (mainCamera)
            {
                fallbackCamera.enabled = false;
            }
            else
            {
                fallbackCamera.enabled = true;
                if (!introFix)
                {
                    introFix = IntroFix.Create();
                }
            }

            cameraManager.SetUp(mainCamera ?? fallbackCamera);
            handsManager.SetUp(playerTransform);
        }

        public void Recenter()
        {
            cameraManager.Recenter();
        }

        private void DelayedRecenter()
        {
            cameraManager.Recenter();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10;
            }
            if (!fallbackCamera.enabled && !(mainCamera && mainCamera.enabled))
            {
                SetUp(null, null);
            }
        }
    }
}
