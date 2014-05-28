using System;
using UnityEditor;
using UnityEngine;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(UnityDateTime))]
	public class UnityDateTimeDrawer : BetterPropertyDrawer<UnityDateTime>
	{
		private bool foldout;

		protected override void Code()
		{
			gui.Foldout(fieldInfo.Name, foldout, newValue =>
			{
				if (foldout != newValue)
				{
					foldout = newValue;
					gui.HeightHasChanged();
				}
			});

			if (!foldout) return;

			var value = TypedValue;

			// Year/Month/Day
			Action ymd = () =>
			{
				gui.HorizontalBlock(() =>
				{
					gui.IntField("Year", value.Year, newYear =>
					{
						if (newYear != value.Year)
							Change(() => value.Year = newYear, "New year");
					});
					gui.IntField("Month", value.Month, newMonth =>
					{
						if (newMonth != value.Month)
							Change(() => value.Month = newMonth, "New month");
					});
					gui.IntField("Day", value.Day, newDay =>
					{
						if (newDay != value.Day)
							Change(() => value.Day = newDay, "New day");
					});
				});
			};

			// Hour/Minute/Second
			Action hms = () =>
			{
				gui.HorizontalBlock(() =>
				{
					gui.IntField("Hour", value.Hour, newHour =>
					{
						if (newHour != value.Hour)
							Change(() => value.Hour = newHour, "New hour");
					});
					gui.IntField("Minute", value.Minute, newMinute =>
					{
						if (newMinute != value.Minute)
							Change(() => value.Minute = newMinute, "New minute");
					});
					gui.IntField("Second", value.Second, newSecond =>
					{
						if (newSecond != value.Second)
							Change(() => value.Second = newSecond, "New second");
					});
				});
			};

			gui.VerticalBlock(GUI.skin.box, () =>
			{
				gui.LabelWidthBlock(50f, (Action)(ymd + hms));

				gui.HorizontalBlock(() =>
				{
					gui.Label("Now", new EditorGUIFramework.GUIOption { Width = 46f });
					gui.TextFieldLabel(UnityDateTime.Now.ToString());
				});
			});
		}

		private void Change(Action change, string whatHasChanged)
		{
			Undo.RecordObject(target, whatHasChanged);
			change();
		}
	}
}