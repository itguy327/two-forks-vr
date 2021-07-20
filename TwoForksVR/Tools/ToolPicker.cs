﻿using System;
using System.Linq;
using TwoForksVR.Assets;
using UnityEngine;
using Valve.VR;

namespace TwoForksVR.Tools
{
	public class ToolPicker : MonoBehaviour
	{
		private const float minSquareDistance = 0.03f;

		private Transform toolsContainer;
		private ToolPickerItem[] tools;
		private SteamVR_Action_Boolean input = SteamVR_Actions.default_ToolPicker;
		private ToolPickerItem hoveredTool;
		private ToolPickerItem selectedTool;
		private Transform rightHand;
		private Transform leftHand;

		public static ToolPicker Create(Transform parent, Transform leftHand, Transform rightHand)
        {
			var instance = Instantiate(VRAssetLoader.ToolPicker).AddComponent<ToolPicker>();
			instance.transform.SetParent(parent, false);
			instance.toolsContainer = instance.transform.Find("Tools");
			instance.rightHand = rightHand;
			instance.leftHand = leftHand;

			instance.tools = instance.toolsContainer.Cast<Transform>().Select(
				(child, index) => ToolPickerItem.Create(
					parent: instance.toolsContainer,
					index: index
				)
			).ToArray();

			return instance;
		}

		private void Start()
		{
			CloseToolPicker();
		}

		private void SelectCurrentlyHoveredTool()
		{
			if (!hoveredTool) return;

			hoveredTool.Select();
			hoveredTool.EndHover();
			selectedTool = hoveredTool;
			hoveredTool = null;
		}

		private void DeselectCurrentlySelectedTool()
		{
			if (!selectedTool) return;

			selectedTool.Deselect();
			selectedTool = null;
		}

		private void OpenToolPicker(Transform hand)
		{
			if (toolsContainer.gameObject.activeSelf) return;

			toolsContainer.gameObject.SetActive(true);
			toolsContainer.position = hand.position;
			toolsContainer.LookAt(Camera.main.transform);
			DeselectCurrentlySelectedTool();
		}

		private void CloseToolPicker()
		{
			if (!toolsContainer.gameObject.activeSelf) return;

			toolsContainer.gameObject.SetActive(false);
			toolsContainer.SetParent(transform);
			toolsContainer.localPosition = Vector3.zero;
			toolsContainer.localRotation = Quaternion.identity;
			SelectCurrentlyHoveredTool();
		}

		private void UpdateSelectedTool(Transform hand)
		{
			ToolPickerItem nextSelectedTool = null;
			var selectedToolDistance = Mathf.Infinity;

			for (int i = 0; i < tools.Length; i++)
			{
				var tool = tools[i];
				var distance = MathHelper.GetSquareDistance(tool.transform, hand);
				if (distance < minSquareDistance && distance < selectedToolDistance)
				{
					nextSelectedTool = tool;
					selectedToolDistance = distance;
				}
			}
			if (nextSelectedTool == hoveredTool)
			{
				return;
			}
			if (hoveredTool)
			{
				hoveredTool.EndHover();
				hoveredTool = null;
			}
			if (nextSelectedTool)
			{
				hoveredTool = nextSelectedTool;
				nextSelectedTool.StartHover();
			}
		}

		private void Update()
		{
			if (input.GetState(SteamVR_Input_Sources.RightHand))
			{
				UpdateSelectedTool(rightHand);
			}
			if (input.GetState(SteamVR_Input_Sources.LeftHand))
			{
				UpdateSelectedTool(leftHand);
			}
			if (input.GetStateDown(SteamVR_Input_Sources.RightHand))
			{
				OpenToolPicker(rightHand);
			}
			if (input.GetStateDown(SteamVR_Input_Sources.LeftHand))
			{
				OpenToolPicker(leftHand);
			}
			if (input.stateUp)
			{
				CloseToolPicker();
			}
		}
	}
}