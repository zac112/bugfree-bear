using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using Object = UnityEngine.Object;

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
						if (!isNull)
						{
							gui.Foldout(foldout, f => foldout = f);
							gui.Space(-10f);
						}
						gui.PropertyField(property, label);
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