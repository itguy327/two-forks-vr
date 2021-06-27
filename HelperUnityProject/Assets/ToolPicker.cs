﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Raicuparta.TwoForksVR.UnityHelper
{
	public class ToolPicker : MonoBehaviour
	{
		public enum VRToolItem
		{
			Radio,
			Map,
			Compass,
			Flashlight,
		}

		public Transform ParentWhileActive;
		public Transform ParentWhileInactive;
		public Transform ToolsContainer;
		public Transform Hand;
		public SteamVR_Input_Sources InputSource;
		public Action<VRToolItem> OnSelectItem;
		public Action<VRToolItem> OnDeselectItem;

		private const float circleRadius = 0.25f;
		private const float minSquareDistance = 0.03f;

		private ToolPickerItem[] tools;
		private SteamVR_Action_Boolean input = SteamVR_Actions.default_ToolPicker;
		private ToolPickerItem hoveredTool;
		private ToolPickerItem selectedTool;

		private void Start()
		{
			SetUpToolsList();
		}

		private void SetUpToolsList()
		{
			tools = ToolsContainer.GetComponentsInChildren<ToolPickerItem>();
			for (var i = 0; i < tools.Length; i++)
			{
				var tool = tools[i];
				float angle = i * Mathf.PI * 2f / tools.Length;
				tool.transform.localPosition = new Vector3(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius, 0);
			}
		}

		private float GetSquareDistance(Vector3 pointA, Vector3 pointB)
		{
			return (pointA - pointB).sqrMagnitude;
		}

		private float GetDistanceToHand(Transform compareTransform)
		{
			return GetSquareDistance(compareTransform.position, Hand.position);
		}

		private void DeselectCurrentlySelectedTool()
		{
			if (!selectedTool || OnDeselectItem == null) return;

			OnDeselectItem(selectedTool.ItemType);
			selectedTool = null;
		}

		private void SelectCurrentlyHoveredTool()
		{
			if (!hoveredTool || OnSelectItem == null) return;

			OnSelectItem(hoveredTool.ItemType);
			hoveredTool.EndHover();
			selectedTool = hoveredTool;
			hoveredTool = null;
		}

		private void OpenToolPicker()
		{
			if (ToolsContainer.gameObject.activeSelf) return;

			Debug.Log("OpenToolPicker");

			ToolsContainer.gameObject.SetActive(true);
			ToolsContainer.SetParent(ParentWhileActive);
			ToolsContainer.LookAt(Camera.main.transform);
			DeselectCurrentlySelectedTool();
		}

		private void CloseToolPicker()
		{
			if (!ToolsContainer.gameObject.activeSelf) return;

			Debug.Log("CloseToolPicker");

			ToolsContainer.gameObject.SetActive(false);
			ToolsContainer.SetParent(ParentWhileInactive);
			ToolsContainer.localPosition = Vector3.zero;
			ToolsContainer.localRotation = Quaternion.identity;
			SelectCurrentlyHoveredTool();
		}

		private void UpdateSelectedTool()
		{
			ToolPickerItem nextSelectedTool = null;
			var selectedToolDistance = Mathf.Infinity;

			for (int i = 0; i < tools.Length; i++)
			{
				var tool = tools[i];
				var distance = GetDistanceToHand(tool.transform);
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
			if (input.GetState(InputSource))
			{
				UpdateSelectedTool();
			}
			if (input.GetStateDown(InputSource))
			{
				OpenToolPicker();
			}
			if (input.GetStateUp(InputSource))
			{
				CloseToolPicker();
			}
		}
	}
}