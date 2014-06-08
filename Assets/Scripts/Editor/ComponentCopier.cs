using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using Vexe.EditorHelpers;
using Vexe.RuntimeExtensions;
using System.Linq;
using UnityEditorInternal;

public class ComponentCopier : EditorWindow
{
	private GLWrapper gui = new GLWrapper();
	private GameObject source;
	private GameObject destination;

	[MenuItem("Vexe/ComponentCopier")]
	public static void ShowMenu()
	{
		GetWindow<ComponentCopier>();
	}

	private void OnGUI()
	{
		gui.LabelWidthBlock(75f, () =>
		{
			gui.HorizontalBlock(() =>
			{
				gui.ObjectField("Source", source, newGo => source = newGo);
				gui.MiniButton("S", "Set to current selection", () => source = Selection.activeGameObject);

				gui.EnabledBlock(source != null, () =>
					gui.MiniButton("C", "Clears source gameObject", MiniButtonStyle.Right, source.ClearComponents)
				);
			});

			gui.HorizontalBlock(() =>
			{
				gui.ObjectField("Destination", destination, newGo => destination = newGo);
				gui.MiniButton("S", "Set to current selection", () => destination = Selection.activeGameObject);
				gui.EnabledBlock(destination != null, () =>
					gui.MiniButton("C", "Clears destination gameObject", MiniButtonStyle.Right, destination.ClearComponents)
				);
			});
		});

		gui.FlexibleSpace();

		gui.Button("Copy And Paste", () =>
		{
			var all = source.GetAllComponents();

			// The first is the Transform component
			ComponentUtility.CopyComponent(all[0]);
			ComponentUtility.PasteComponentValues(destination.transform);

			// We skip the first and copy/paste the rest as new components
			foreach (var c in all.Skip(1))
			{
				ComponentUtility.CopyComponent(c);
				ComponentUtility.PasteComponentAsNew(destination);
			}
		});
	}
}