using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using Object = UnityEngine.Object;
using System;

namespace ShowEmAll.DrawMates
{
	public class InlineDrawer : BaseDrawer<GLWrapper, GLOption>
	{
		private Editor editor;

		public SerializedProperty property { get; set; }
		public string label { get; set; }

		public InlineDrawer(GLWrapper gui, Object target)
			: base(gui, target)
		{
		}

		protected override string key { get { return base.key + label; } }

		public override void Draw()
		{
			gui.VerticalBlock(() =>
			{
				gui.ApplyAfterChange(property.serializedObject, () =>
				{
					var objRef = property.objectReferenceValue;
					bool isNull = objRef == null;
					gui.HorizontalBlock(() =>
					{
						try
						{
							if (!isNull)
							{
								bool current = foldout;
								gui.Foldout(current, f =>
								{
									if (current != f)
										foldout = f;
								});
								gui.Space(-10f);
							}
							gui.PropertyField(property, label);
						}
						catch (ArgumentException e)
						{
							// Sometimes I get:
							// ArgumentException: Getting control 1's position in a group with only 1 controls when doing Repaint
							// This is due to the fact that GUI code gets executed more than once each frame
							// The first pass is the Layout event, the second is Repaint
							// This error occur when a control, say GUILayout.Label is executed in the Repaint event but not in the Layout
							// so the GUI will complain that it doesn't have a Layout entry for this control thus don't know how to draw it...
							// In our case, the condition !isNull might be true for Repaint, but not Layout
							// so the GUI might throw from gui.Space or gui.PropertyField
						}
					});

					if (foldout && !isNull)
					{
						if (editor == null)
						{
							editor = Editor.CreateEditor(objRef);
						}

						gui.VerticalBlock(GUI.skin.box, () =>
						{
							if (objRef is GameObject)
							{
								gui.HorizontalBlock(() =>
								{
									// indenting/spacing doesn't make the header go to the right
									// cause they hardcoded the coordinates to start drawing at 3 x
									// Unity :)
									editor.DrawHeader();
									EditorGUIUtility.labelWidth = 0;
								});
							}
							else
							{
								editor.OnInspectorGUI();
							}
						});
					}
				});
			});
		}
	}
}