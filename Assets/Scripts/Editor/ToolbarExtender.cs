using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[InitializeOnLoad]
	public static class ToolbarExtender
	{
		private static readonly int MToolCount;
		private static GUIStyle _mCommandStyle;
		
		public static readonly List<Action> RightToolbarGUI = new();

		static ToolbarExtender()
		{
			var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
			
			const string fieldName = "k_ToolCount";
			
			var toolIcons = toolbarType.GetField(fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			
			MToolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 8;
	
			ToolbarCallback.OnToolbarGUI = OnGUI;
			ToolbarCallback.OnToolbarGUILeft = GUILeft;
			ToolbarCallback.OnToolbarGUIRight = GUIRight;
		}

		private const float Space = 8;
		
		private const float ButtonWidth = 32;
		private const float DropdownWidth = 80;

		private const float PlayPauseStopWidth = 140;


		static void OnGUI()
		{
			// Create two containers, left and right
			// Screen is whole toolbar

			_mCommandStyle ??= new GUIStyle("CommandLeft");

			var screenWidth = EditorGUIUtility.currentViewWidth;

			// Following calculations match code reflected from Toolbar.OldOnGUI()
			float playButtonsPosition = Mathf.RoundToInt ((screenWidth - PlayPauseStopWidth) / 2);

			var leftRect = new Rect(0, 0, screenWidth, Screen.height);
			leftRect.xMin += Space; // Spacing left
			leftRect.xMin += ButtonWidth * MToolCount; // Tool buttons

			leftRect.xMin += Space; // Spacing between tools and pivot

			leftRect.xMin += 64 * 2; // Pivot buttons
			leftRect.xMax = playButtonsPosition;

			var rightRect = new Rect(0, 0, screenWidth, Screen.height)
			{
				xMin = playButtonsPosition
			};
			
			rightRect.xMin += _mCommandStyle.fixedWidth * 3; // Play buttons
			rightRect.xMax = screenWidth;
			rightRect.xMax -= Space; // Spacing right
			rightRect.xMax -= DropdownWidth; // Layout
			rightRect.xMax -= Space; // Spacing between layout and layers
			rightRect.xMax -= DropdownWidth; // Layers

			rightRect.xMax -= Space; // Spacing between layers and account

			rightRect.xMax -= DropdownWidth; // Account
			rightRect.xMax -= Space; // Spacing between account and cloud
			rightRect.xMax -= ButtonWidth; // Cloud
			rightRect.xMax -= Space; // Spacing between cloud and collab
			rightRect.xMax -= 78; // Colab

			// Add spacing around existing controls
			leftRect.xMin += Space;
			leftRect.xMax -= Space;
			rightRect.xMin += Space;
			rightRect.xMax -= Space;

			// Add top and bottom margins
			leftRect.y = 4;
			leftRect.height = 22;
			rightRect.y = 4;
			rightRect.height = 22;

			if (leftRect.width > 0)
			{
				GUILayout.BeginArea(leftRect);
				GUILayout.BeginHorizontal();

				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}

			if (!(rightRect.width > 0)) 
				return;
			
			GUILayout.BeginArea(rightRect);
			GUILayout.BeginHorizontal();
			
			foreach (var handler in RightToolbarGUI)
				handler();

			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
		}

		private static void GUILeft() {
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();
		}

		private static void GUIRight() {
			GUILayout.BeginHorizontal();
			foreach (var handler in RightToolbarGUI)
			{
				handler();
			}
			GUILayout.EndHorizontal();
		}
	}
}
