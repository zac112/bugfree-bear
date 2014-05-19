﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor: Editor{

	GameEvent _target;
	MutableGameTime eventStart;
	MutableGameTime eventStop;
	GUIContent dayContent = new GUIContent("Day:","Day 11600 is the practical limit due to floating point limitations.");

	void OnEnable(){
		_target = (GameEvent)target;
		if(_target.endTime == null || _target.startTime == null){
			_target.startTime = new MutableGameTime(new GameTime(0f));
			_target.endTime = new MutableGameTime(new GameTime(0f));
		}
		eventStart = new MutableGameTime(_target.startTime);
		eventStop = new MutableGameTime(_target.endTime);
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

		_target.startTime = (GameTime)eventStart;
		_target.endTime = (GameTime)eventStop;
	}

	private void Sliders(MutableGameTime time){
		int day = Mathf.Max(0,EditorGUILayout.IntField(dayContent,time.Day));
		int hour = EditorGUILayout.IntSlider("Hour:",time.Hours,0,23);
		int minute = EditorGUILayout.IntSlider("Minute:",time.Minutes,0,59);
		
		float gameTime = day*1440+hour*60f+minute;
		time.SinceBeginning = gameTime;
	}

	[System.Serializable]
	private class MutableGameTime : GameTime{
		
		public MutableGameTime(GameTime gameTime) : base(gameTime.SinceBeginning){
			
		}
		
		public new float SinceBeginning{
			get{ return (float)time; }
			set{ time = value; }
		}
	}
}
