using UnityEngine;
using UnityEditor;
using System;
using sp = UnityEditor.SerializedProperty;
using EditorGUIFramework;
using ShowEmAll.DrawMates;
using uFAction.Editors;

namespace uFAction
{
	[CustomPropertyDrawer(typeof(ShowDelegate))]
	public class ShowDelegateAttributeDrawer : BetterPropertyDrawer<ShowDelegate>
	{
		private DelegateDrawer<GUIWrapper, GUIOption> drawer;

		protected override void Code()
		{
			drawer.Draw();
		}

		protected override void Init(sp property, GUIContent label)
		{
			base.Init(property, label);
			drawer = new DelegateDrawer<GUIWrapper, GUIOption>(gui)
			{
				spDelegate = property,
				delegateObject = fieldInfo.GetValue(target),
				title = TypedValue.title,
				canSetArgsFromEditor = TypedValue.canSetArgsFromEditor,
				forceExpand = TypedValue.forceExpand
			};
		}
	}

	public class DelegateDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private EditorViewStyle? prevStyle;
		private BaseEditor<TWrapper, TOption> currentEditor;

		public SerializedProperty spDelegate { get; set; }
		public object delegateObject { get; set; }
		public string title { get; set; }
		public bool canSetArgsFromEditor { get; set; }
		public bool forceExpand { get; set; }

		public DelegateDrawer(TWrapper gui)
		{
			this.gui = gui;
		}

		private EditorViewStyle ViewStyle
		{
			get { return (delegateObject as IViewableDelegate).CurrentViewStyle; }
		}

		public override void Draw()
		{
			CheckEditorType();
			//currentEditor.DrawTitleHeader(forceExpand);
			currentEditor.Draw();
		}

		private void CheckEditorType()
		{
			var currentStyle = ViewStyle;
			if (!prevStyle.HasValue || prevStyle != currentStyle)
			{
				SetEditor(Type.GetType(typeof(BaseEditor<,>).Namespace + "." + currentStyle + "Editor`2").MakeGenericType(typeof(TWrapper), typeof(TOption)));
				prevStyle = currentStyle;
			}
		}

		private void SetEditor(Type editorType)
		{
			currentEditor = Activator.CreateInstance(editorType) as BaseEditor<TWrapper, TOption>;
			currentEditor.Set(spDelegate, delegateObject, title, canSetArgsFromEditor, forceExpand, gui);
		}
	}
}