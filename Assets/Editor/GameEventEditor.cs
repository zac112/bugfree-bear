using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor: Editor{

	int day = 0;
	int hour = 0;
	int minute = 0;
	GameEvent _target;
	GameTime eventStart;

	void OnEnable(){
		_target = (GameEvent)target;
		eventStart = _target.startTime;
	}

	public override void OnInspectorGUI(){
		EditorGUI.BeginChangeCheck();
		{
			day = Mathf.Max(0,EditorGUILayout.IntField("Day:",eventStart.Day));
			hour = EditorGUILayout.IntSlider("Hour:",eventStart.Hours,0,23);
			minute = EditorGUILayout.IntSlider("Minute:",eventStart.Minutes,0,59);
		}if(EditorGUI.EndChangeCheck()){
			eventStart.SinceBeginning = day*1440+hour*60f+minute;
		}
	}
}
