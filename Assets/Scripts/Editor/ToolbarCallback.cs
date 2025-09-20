using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
	public static class ToolbarCallback
	{
		private static readonly Type MToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

		private static ScriptableObject _mCurrentToolbar;

		/// <summary>
		/// Callback for toolbar OnGUI method.
		/// </summary>
		public static Action OnToolbarGUI;
		public static Action OnToolbarGUILeft;
		public static Action OnToolbarGUIRight;
		
		static ToolbarCallback()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			// Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
			if (_mCurrentToolbar != null)
				return;
			
			// Find toolbar
			var toolbars = Resources.FindObjectsOfTypeAll(MToolbarType);
			_mCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
			if (_mCurrentToolbar == null) 
				return;
			
			var root = _mCurrentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
			if (root == null) 
				return;
					
			var rawRoot = root.GetValue(_mCurrentToolbar);
			var mRoot = rawRoot as VisualElement;
			RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
			RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);
			return;

			void RegisterCallback(string rootInner, Action cb) {
				var toolbarZone = mRoot.Q(rootInner);

				var parent = new VisualElement()
				{
					style = {
						flexGrow = 1,
						flexDirection = FlexDirection.Row,
					}
				};
				var container = new IMGUIContainer();
				container.style.flexGrow = 1;
				container.onGUIHandler += () => { 
					cb?.Invoke();
				}; 
				parent.Add(container);
				toolbarZone.Add(parent);
			}
		}

		static void OnGUI()
		{
			var handler = OnToolbarGUI;
			handler?.Invoke();
		}
	}
}
