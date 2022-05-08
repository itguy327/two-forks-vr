using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using LIV.SDK.Unity;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr;

[BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "1.0.0")]
public class TwoForksVrMod : BaseUnityPlugin
{
    private void Awake()
    {
        VrSettings.SetUp(Config);
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        VrAssetLoader.LoadAssets();
        
        Logs.WriteInfo($"shader is loaded {VrAssetLoader.LivShadersBundle}");
        SDKShaders.LoadFromAssetBundle(VrAssetLoader.LivShadersBundle);
        InitSteamVR();
    }

    private static void InitSteamVR()
    {
        SteamVR_Actions.PreInitialize();
        SteamVR.Initialize();
        SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;
    }
}