using UnityEngine;
using EditorGUIFramework;
using System;
using Vexe.RuntimeExtensions;
using sp = UnityEditor.SerializedProperty;
using Object = UnityEngine.Object;

namespace ShowEmAll.DrawMates
{
	public class TransformDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private Vector3Drawer<TWrapper, TOption> posDrawer;
		private Vector3Drawer<TWrapper, TOption> rotDrawer;
		private Vector3Drawer<TWrapper, TOption> scaleDrawer;

		public sp spPos { get; set; }
		public sp spRot { get; set; }
		public sp spScale { get; set; }

		public TransformDrawer(TWrapper gui, Object target)
			: base(gui, target)
		{
			posDrawer = new Vector3Drawer<TWrapper, TOption>("Position", gui, target);
			rotDrawer = new Vector3Drawer<TWrapper, TOption>("Rotation", gui, target);
			scaleDrawer = new Vector3Drawer<TWrapper, TOption>("Scale", gui, target);
		}

		public override void Draw()
		{
			posDrawer.Draw(spPos);
			Action<Vector3> setQuat = eulerAngles =>
			{
				if (!eulerAngles.ApproxEqual(spRot.quaternionValue.eulerAngles))
				{
					var q = new Quaternion();
					q.eulerAngles = eulerAngles;
					spRot.quaternionValue = q;
				}
			};
			rotDrawer.Draw(() => spRot.quaternionValue.eulerAngles, setQuat);
			scaleDrawer.Draw(spScale);
		}
	}
}