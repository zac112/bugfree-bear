using UnityEditor;
using Vexe.EditorHelpers;
using Vexe.EditorExtensions;
using Vexe.RuntimeHelpers;
using EditorGUIFramework;
using UnityEngine;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ReadonlyAttribute))]
	public class ReadonlyAttributeDrawer : BetterPropertyDrawer<ReadonlyAttribute>
	{
		protected override void Code()
		{
			if (!Application.isPlaying && TypedValue.AssignAtEditTime)
			{
				gui.PropertyField(property, label);
				return;
			}

			var value = property.GetValue();
			bool isNull = value == null;
			gui.HorizontalBlock(() =>
			{
				gui.ColorBlock(GuiHelper.RedColorDuo.SecondColor, () =>
					gui.ReadonlyPropertyField(property, label)
				);
			});
			if (property.propertyType == SerializedPropertyType.ObjectReference && !isNull)
			{
				gui.GetLastRect(lastRect =>
				{
					var @ref = property.objectReferenceValue;
					GuiHelper.PingField(lastRect, @ref, MouseCursor.Link);
					GuiHelper.SelectField(lastRect, @ref, EventsHelper.MouseEvents.IsRMB_MouseDown());
				});
			}
		}
	}
}