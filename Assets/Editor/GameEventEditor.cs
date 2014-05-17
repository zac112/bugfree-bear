using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor: Editor{

	GameTime eventStart;
	GameTime eventStop;
	GUIContent dayContent = new GUIContent("Day:","The day this event should start. Day 11600 is the practical limit due to floating point limitations.");

	void OnEnable(){
		GameEvent _target = (GameEvent)target;
		eventStart = _target.startTime;
		eventStop = _target.endTime;
	}

	public override void OnInspectorGUI(){
		GUILayout.Label("Event start time");
		Sliders(eventStart);

		if(eventStop<eventStart)
			eventStop.SinceBeginning = eventStart.SinceBeginning;

		GUILayout.Label("Event stop time");
		Sliders(eventStop);

		if(eventStop<eventStart)
			eventStart.SinceBeginning = eventStop.SinceBeginning;

	}

	private void Sliders(GameTime time){
		int day = Mathf.Max(0,EditorGUILayout.IntField(dayContent,time.Day));
		int hour = EditorGUILayout.IntSlider("Hour:",time.Hours,0,23);
		int minute = EditorGUILayout.IntSlider("Minute:",time.Minutes,0,59);
		
		float gameTime = day*1440+hour*60f+minute;
		time.SinceBeginning = gameTime;
	}
}
