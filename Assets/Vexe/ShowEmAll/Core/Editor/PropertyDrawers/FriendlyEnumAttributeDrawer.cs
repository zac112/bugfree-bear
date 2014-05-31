using UnityEditor;
using EditorGUIFramework;
using UnityEngine;
using Vexe.RuntimeHelpers;
using Vexe.EditorHelpers;
using Vexe.RuntimeExtensions;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(FriendlyEnumAttribute))]
	public class FriendlyEnumAttributeDrawer : BetterPropertyDrawer<FriendlyEnumAttribute>
	{
		protected override void Code()
		{
			gui.PropertyField(property, label);
			gui.GetLastRect(lastRect =>
			{
				var e = Event.current;
				if (e == null) return;
				if (lastRect.Contains(e.mousePosition) && EventsHelper.MouseEvents.IsRMB_MouseDown())
				{
					string[] names = property.enumNames;
					SelectionWindow.Show<string>(
						@getValues: () => names,
						@getTarget: () => names[property.enumValueIndex],
						@setTarget: name =>
						{
							if (names[property.enumValueIndex] != name)
							{
								property.enumValueIndex = names.IndexOf(name); ;
								property.serializedObject.ApplyModifiedProperties();
							}
						},
						@getValueName: name => name,
						@label: fieldInfo.FieldType.Name + "s"
					);
				}
			});
		}
	}
}