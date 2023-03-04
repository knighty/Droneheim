using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Droneheim
{
	internal class SplineEditorInput
	{
		public KeyboardShortcut toggle = new KeyboardShortcut(UnityEngine.KeyCode.U);
		public KeyboardShortcut addKeyframe = new KeyboardShortcut(UnityEngine.KeyCode.I);
		public KeyboardShortcut playPause = new KeyboardShortcut(UnityEngine.KeyCode.K);
		public KeyboardShortcut rewind = new KeyboardShortcut(UnityEngine.KeyCode.J);
		public KeyboardShortcut forward = new KeyboardShortcut(UnityEngine.KeyCode.L);

		public KeyboardShortcut previousKeyframe = new KeyboardShortcut(UnityEngine.KeyCode.LeftCurlyBracket);
		public KeyboardShortcut nextKeyframe = new KeyboardShortcut(UnityEngine.KeyCode.RightCurlyBracket);
		public KeyboardShortcut delete = new KeyboardShortcut(UnityEngine.KeyCode.Delete);
	}

	internal class SplineEditor
	{
		private bool isActive = false;
		private SplineEditorInput input = new SplineEditorInput();

		public void HandleInput()
		{
			if (input.toggle.IsDown())
			{
				isActive = !isActive;
			}
		}

		public void Enable()
		{

		}

		public void Disable()
		{

		}
	}

	internal class SplineEditorController
	{
		private SplineEditor model;

		public SplineEditorController(SplineEditor model)
		{
			this.model = model;
		}

		public void Show()
		{
		}
	}

	internal class SplineEditorView
	{
		public Rect windowRect = new Rect(20, 20, 1000, 500);

		private SplineEditor model;
		private SplineEditorController controller;

		public SplineEditorView(SplineEditor model, SplineEditorController controller) {
			this.model = model;
			this.controller = controller;
		}

		/*public void Render()
		{
			windowRect = GUI.Window(0, windowRect, DoMyWindow, "My Window");
		}

		// Make the contents of the window
		void DoMyWindow(int windowID)
		{
			if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
			{
			}

			GUI.DragWindow();

			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			for(int i = 0; i < 20; i++)
			{
				GUILayout.Label(i.ToString());
			}
			GUILayout.EndHorizontal();

			//GUI.Box(new Rect(20, 20, 100, 100));
		}*/
	}
}
