using UnityEngine;
using UnityEditor;
using ShowEmAll.DrawMates;
using EditorGUIFramework;
using Vexe.RuntimeHelpers;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ShowTransformAttribute))]
	public class ShowTransformAttributeDrawer : BetterPropertyDrawer<ShowTransformAttribute>
	{
		private string key
		{
			get { return RTHelper.GetTargetID(target) + fieldInfo.Name; }
		}
		private bool foldout
		{
			get { return StuffHelper.GetFoldoutValue(key); }
			set { StuffHelper.SetFoldoutValue(key, value); }
		}

		private bool hasInit;
		private TransformDrawer<GUIWrapper, GUIOption> transformDrawer;
		private SerializedProperty spPos;
		private SerializedProperty spRot;
		private SerializedProperty spScale;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			if (property.type != "PPtr<$Transform>")
				throw new Vexe.RuntimeHelpers.Exceptions.InvalidTypeException("field must be of type UnityEngine.Transform");

			base.Init(property, label);
		}

		protected override void Code()
		{
			if (!hasInit && property.objectReferenceValue != null)
			{
				hasInit = true;
				StuffHelper.InitTransformSPs(property, out spPos, out spRot, out spScale);
				transformDrawer = new TransformDrawer<GUIWrapper, GUIOption>(gui, target)
				{
					spPos = spPos,
					spRot = spRot,
					spScale = spScale
				};
			}

			gui.HorizontalBlock(() =>
			{
				ApplyAfterChange(() => gui.PropertyField(property));
				gui.Space(5f);
				gui.Foldout("", foldout, new GUIOption { Width = 5 }, newValue =>
				{
					if (foldout != newValue)
					{
						foldout = newValue;
						gui.HeightHasChanged();
					}
				});
			});

			if (foldout && property.objectReferenceValue != null)
			{
				gui.VerticalBlock(GUI.skin.box, transformDrawer.Draw);
			}
		}
	}
}