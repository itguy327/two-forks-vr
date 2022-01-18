﻿//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Displays text and button hints on the controllers
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

//-------------------------------------------------------------------------
public class VrButtonHighlight : MonoBehaviour
{
    public Material controllerMaterial;
    public Color flashColor = new Color(1.0f, 0.557f, 0.0f);
    private readonly Dictionary<string, Transform> componentTransformMap = new Dictionary<string, Transform>();
    private readonly List<MeshRenderer> flashingRenderers = new List<MeshRenderer>();
    private readonly List<MeshRenderer> renderers = new List<MeshRenderer>();
    private Dictionary<ISteamVR_Action_In_Source, ActionHintInfo> actionHintInfos;
    private int colorID;
    private SteamVR_Input_Sources inputSource;
    private Transform player;
    private SteamVR_RenderModel renderModel;
    private SteamVR_Events.Action renderModelLoadedAction;
    private float startTime;
    private Transform textHintParent;
    private float tickCount;
    public Material usingMaterial
    {
        get
        {
            return controllerMaterial;
        }
    }

    public bool initialized { get; private set; }

    //-------------------------------------------------
    private void Awake()
    {
        renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(OnRenderModelLoaded);
        renderModel = GetComponent<SteamVR_RenderModel>();
        colorID = Shader.PropertyToID("_Color");
    }

    //-------------------------------------------------
    private void Update()
    {
        if (renderModel != null && renderModel.gameObject.activeInHierarchy && flashingRenderers.Count > 0)
        {
            var baseColor = usingMaterial.GetColor(colorID);

            var flash = (Time.realtimeSinceStartup - startTime) * Mathf.PI * 2.0f;
            flash = Mathf.Cos(flash);
            flash = Util.RemapNumberClamped(flash, -1.0f, 1.0f, 0.0f, 1.0f);

            var ticks = Time.realtimeSinceStartup - startTime;
            if (ticks - tickCount > 1.0f)
            {
                tickCount += 1.0f;
            }

            for (var i = 0; i < flashingRenderers.Count; i++)
            {
                Renderer r = flashingRenderers[i];
                r.material.SetColor(colorID, Color.Lerp(baseColor, flashColor, flash));
            }
        }
    }


    //-------------------------------------------------
    private void OnEnable()
    {
        renderModelLoadedAction.enabled = true;
    }


    //-------------------------------------------------
    private void OnDisable()
    {
        renderModelLoadedAction.enabled = false;
        Clear();
    }

    private void ShowHintsAll()
    {
        ShowButtonHint(SteamVR_Input.actionsIn);
    }


    //-------------------------------------------------
    private void OnParentHandInputFocusLost()
    {
        //Hide all the hints when the controller is no longer the primary attached object
        HideAllButtonHints();
    }


    public virtual void SetInputSource(SteamVR_Input_Sources newInputSource)
    {
        inputSource = newInputSource;
        if (renderModel != null)
            renderModel.SetInputSource(newInputSource);
    }

    //-------------------------------------------------
    // Gets called when the hand has been initialized and a render model has been set
    //-------------------------------------------------
    private void OnHandInitialized(int deviceIndex)
    {
        //Create a new render model for the controller hints
        renderModel = new GameObject("SteamVR_RenderModel").AddComponent<SteamVR_RenderModel>();
        renderModel.transform.parent = transform;
        renderModel.transform.localPosition = Vector3.zero;
        renderModel.transform.localRotation = Quaternion.identity;
        renderModel.transform.localScale = Vector3.one;

        renderModel.SetInputSource(inputSource);
        renderModel.SetDeviceIndex(deviceIndex);

        if (!initialized)
            //The controller hint render model needs to be active to get accurate transforms for all the individual components
            renderModel.gameObject.SetActive(true);
    }

    //-------------------------------------------------
    private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool succeess)
    {
        //Only initialize when the render model for the controller hints has been loaded
        if (renderModel == this.renderModel)
        {
            //Debug.Log("<b>[SteamVR Interaction]</b> OnRenderModelLoaded: " + this.renderModel.renderModelName);
            if (initialized)
            {
                Destroy(textHintParent.gameObject);
                componentTransformMap.Clear();
                flashingRenderers.Clear();
            }

            renderModel.SetMeshRendererState(false);

            StartCoroutine(DoInitialize(renderModel));
        }
    }

    private IEnumerator DoInitialize(SteamVR_RenderModel renderModel)
    {
        while (renderModel.initializedAttachPoints == false)
            yield return null;

        textHintParent = new GameObject("Text Hints").transform;
        textHintParent.SetParent(transform);
        textHintParent.localPosition = Vector3.zero;
        textHintParent.localRotation = Quaternion.identity;
        textHintParent.localScale = Vector3.one;

        //Get the button mask for each component of the render model

        var renderModels = OpenVR.RenderModels;
        if (renderModels != null)
        {
            for (var childIndex = 0; childIndex < renderModel.transform.childCount; childIndex++)
            {
                var child = renderModel.transform.GetChild(childIndex);

                if (!componentTransformMap.ContainsKey(child.name))
                {
                    componentTransformMap.Add(child.name, child);
                }
            }
        }

        actionHintInfos = new Dictionary<ISteamVR_Action_In_Source, ActionHintInfo>();

        for (var actionIndex = 0; actionIndex < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; actionIndex++)
        {
            var action = SteamVR_Input.actionsNonPoseNonSkeletonIn[actionIndex];

            if (action.GetActive(inputSource))
                CreateAndAddButtonInfo(action, inputSource);
        }

        initialized = true;

        //Set the controller hints render model to not active
        renderModel.SetMeshRendererState(true);
        renderModel.gameObject.SetActive(false);
    }


    //-------------------------------------------------
    private void CreateAndAddButtonInfo(ISteamVR_Action_In action, SteamVR_Input_Sources inputSource)
    {
        Transform buttonTransform = null;
        var buttonRenderers = new List<MeshRenderer>();

        var buttonDebug = new StringBuilder();
        buttonDebug.Append("Looking for action: ");

        buttonDebug.AppendLine(action.GetShortName());

        buttonDebug.Append("Action localized origin: ");
        buttonDebug.AppendLine(action.GetLocalizedOrigin(inputSource));

        var actionComponentName = action.GetRenderModelComponentName(inputSource);

        if (componentTransformMap.ContainsKey(actionComponentName))
        {
            buttonDebug.AppendLine(string.Format("Found component: {0} for {1}", actionComponentName,
                action.GetShortName()));
            var componentTransform = componentTransformMap[actionComponentName];

            buttonTransform = componentTransform;

            buttonDebug.AppendLine(string.Format("Found componentTransform: {0}. buttonTransform: {1}",
                componentTransform, buttonTransform));

            buttonRenderers.AddRange(componentTransform.GetComponentsInChildren<MeshRenderer>());
        }
        else
        {
            buttonDebug.AppendLine(string.Format(
                "Can't find component transform for action: {0}. Component name: \"{1}\"", action.GetShortName(),
                actionComponentName));
        }

        buttonDebug.AppendLine(string.Format("Found {0} renderers for {1}", buttonRenderers.Count,
            action.GetShortName()));

        foreach (var renderer in buttonRenderers)
        {
            buttonDebug.Append("\t");
            buttonDebug.AppendLine(renderer.name);
        }

        if (buttonTransform == null)
        {
            Debug.Log("Couldn't find buttonTransform for " + action.GetShortName());
            return;
        }

        var hintInfo = new ActionHintInfo();
        actionHintInfos.Add(action, hintInfo);
        hintInfo.renderers = buttonRenderers;
    }

    //-------------------------------------------------
    public void ShowButtonHint(params ISteamVR_Action_In_Source[] actions)
    {
        renderModel.gameObject.SetActive(true);

        renderModel.GetComponentsInChildren(renderers);
        for (var i = 0; i < renderers.Count; i++)
        {
            var mainTexture = renderers[i].material.mainTexture;
            renderers[i].sharedMaterial = usingMaterial;
            renderers[i].material.mainTexture = mainTexture;

            // This is to poke unity into setting the correct render queue for the model
            renderers[i].material.renderQueue = usingMaterial.shader.renderQueue;
        }

        for (var i = 0; i < actions.Length; i++)
            if (actionHintInfos.ContainsKey(actions[i]))
            {
                var hintInfo = actionHintInfos[actions[i]];
                foreach (var renderer in hintInfo.renderers)
                    if (!flashingRenderers.Contains(renderer))
                        flashingRenderers.Add(renderer);
            }

        startTime = Time.realtimeSinceStartup;
        tickCount = 0.0f;
    }


    //-------------------------------------------------
    public void HideAllButtonHints()
    {
        Clear();

        if (renderModel != null && renderModel.gameObject != null)
            renderModel.gameObject.SetActive(false);
    }


    //-------------------------------------------------
    public void HideButtonHint(params ISteamVR_Action_In_Source[] actions)
    {
        var baseColor = usingMaterial.GetColor(colorID);
        for (var i = 0; i < actions.Length; i++)
            if (actionHintInfos.ContainsKey(actions[i]))
            {
                var hintInfo = actionHintInfos[actions[i]];
                foreach (var renderer in hintInfo.renderers)
                {
                    renderer.material.color = baseColor;
                    flashingRenderers.Remove(renderer);
                }
            }

        if (flashingRenderers.Count == 0) renderModel.gameObject.SetActive(false);
    }

    //-------------------------------------------------
    private bool IsButtonHintActive(ISteamVR_Action_In_Source action)
    {
        if (actionHintInfos.ContainsKey(action))
        {
            var hintInfo = actionHintInfos[action];
            foreach (var buttonRenderer in hintInfo.renderers)
                if (flashingRenderers.Contains(buttonRenderer))
                    return true;
        }

        return false;
    }

    //-------------------------------------------------
    private void Clear()
    {
        renderers.Clear();
        flashingRenderers.Clear();
    }

    //Info for each of the buttons
    private class ActionHintInfo
    {
        public List<MeshRenderer> renderers;
    }
}