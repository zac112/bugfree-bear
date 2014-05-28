using UnityEngine;
using UnityEditor;
using sp = UnityEditor.SerializedProperty;
using System;
using Vexe.RuntimeHelpers;
using Vexe.EditorExtensions;
using Vexe.RuntimeExtensions;
using Object = UnityEngine.Object;
using Vexe.EditorHelpers;

namespace ShowEmAll
{
	// TODO: Clean up
	public static class StuffHelper
	{
		public static BetterPrefs BetterPrefs { get { return BetterPrefs.Instance; } }
		public static string GetTargetID(Object target)
		{
			var unique = target as IUniquelyIdentifiedObject;
			return unique == null ? target.GetHashCode().ToString() : unique.ID;
		}
		public static bool GetFoldoutValue(string key)
		{
			return BetterPrefs.GetSafeBool(key);
		}
		public static void SetFoldoutValue(string key, bool value)
		{
			BetterPrefs.SetBool(key, value);
		}

		private static Texture darkKnobBG;
		private static Texture darkKnob;
		private static Texture DarkKnobBG { get { return GetTexture(ref darkKnobBG, "Assets/Textures/Knobs/DarkKnobBG.png"); } }
		private static Texture DarkKnob { get { return GetTexture(ref darkKnob, "Assets/Textures/Knobs/DarkKnob.png"); } }

		private static Texture GetTexture(ref Texture t, string path)
		{
			return null;
			//Func<Texture> getTexture = () =>
			//{
			//	return AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
			//};
			//return RTHelper.LazyValue(() => t, tex => t = tex, getTexture);
		}

		public static float Angle(Rect rect, float value, Action onChange, float snap, bool slider)
		{
			// give an id to our control
			int id = GUIUtility.GetControlID(FocusType.Native, rect);

			// for some reason the width is just too big, so we set it equal to the height
			rect.width = rect.height;

			// we set our center and radius - the radius offset is depedent on the texture you're using
			float offset = 55f;
			var radius = new Vector2(rect.xMin, rect.yMin).Subtract(offset);
			var center = rect.center;

			// take the current event
			var e = Event.current;
			if (e != null)	// only act upon it if it's not null
			{
				// we put our movement inside a delegate, cause we're gonna reuse it
				Action move = () =>
				{
					// cache the value
					float prevValue = value;
					// take the mouse position
					var mouse = e.mousePosition;
					// the direction from the rect's center to the mouse
					var dir = (mouse - center);
					// set the value to the angle between the direction vector and v2.right
					value = Vector2.Angle(dir, Vector2.right);
					// if we're above 180, subtract from 360 to get the complement
					if (Vector2.Dot(dir, Vector2.up) > 0)
					{
						value = 360 - value;
					}

					// fire onChange if the value has changed
					if (!Mathf.Approximately(prevValue, value) && onChange != null)
						onChange();
				};

				// if we have a mouse down, and the mouse is contained within our rect
				if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
				{
					// we set the current active control to our control
					GUIUtility.hotControl = id;
					// and move
					move();
				}
				// if the current active control is ours
				if (GUIUtility.hotControl == id)
				{
					// if we have a mouse up
					if (e.type == EventType.MouseUp)
					{
						// we release our hands from the hotControl trigger - free the active control
						GUIUtility.hotControl = 0;
					}
					// if we have a drag
					else if (e.type == EventType.MouseDrag)
					{
						// we move
						move();
					}
				}
			}

			float fOffset = 10f;
			Rect fRect = new Rect(rect.x + rect.height + fOffset, rect.center.y - fOffset, 50, 18);
			if (slider)
			{
				fRect.width = 275;
				value = EditorGUI.Slider(fRect, value, 0, 360);
			}
			else
			{
				value = EditorGUI.FloatField(fRect, value);
			}

			// we now draw the knob bg
			GUI.DrawTexture(rect, DarkKnobBG);
			// cache the matrix
			Matrix4x4 matrix = GUI.matrix;
			// rotate the gui by the opposite of value around our rect center
			GUIUtility.RotateAroundPivot(-value, rect.center);
			// we set the dimensions and coords of our knob
			var knobSize = 7.5f;
			float x = rect.center.x - radius.x;
			float y = rect.center.y;
			var knobRect = new Rect(x, y, knobSize, knobSize);
			// draw it
			GUI.DrawTexture(knobRect, DarkKnob);
			// reset the gui matrix
			GUI.matrix = matrix;

			return value;
		}


		public const string TRANSFORM_LOCAL_POSITION = "m_LocalPosition";
		public const string TRANSFORM_LOCAL_ROTATION = "m_LocalRotation";
		public const string TRANSFORM_LOCAL_SCALE = "m_LocalScale";

		public static void SerializedObjectBlock(SerializedObject so, Action block)
		{
			so.Update();
			block();
			so.ApplyModifiedProperties();
		}

		public static void InitTransformSPs(SerializedObject obj, out sp spPos, out sp spRot, out sp spScale)
		{
			spPos = obj.FindProperty(TRANSFORM_LOCAL_POSITION);
			spRot = obj.FindProperty(TRANSFORM_LOCAL_ROTATION);
			spScale = obj.FindProperty(TRANSFORM_LOCAL_SCALE);
		}

		public static void InitTransformSPs(SerializedProperty property, out sp spPos, out sp spRot, out sp spScale)
		{
			SerializedObject soTransform;
			spPos = property.FindPropertyRelativeInMB(TRANSFORM_LOCAL_POSITION, out soTransform);
			spRot = soTransform.FindProperty(TRANSFORM_LOCAL_ROTATION);
			spScale = soTransform.FindProperty(TRANSFORM_LOCAL_SCALE);
		}
	}
}