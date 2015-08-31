using UnityEngine;
using UnityEditor;
using System.Collections;
using KyUtils;

[CustomEditor(typeof(Timer))]
public class TimerEditor : Editor {

	private bool bFold = true;

	private string getTimerString(TimerHandle th)
	{
		string pattern = "<size=13>Status:<color={0}>{1}</color>\tTag:{2} </size>";
		return string.Format(pattern, 
		                     th.bPaused? "red" : "green", 
		                     th.bPaused? "Paused":"Running",
		                     th.tag
		                     );

	}

	public override void OnInspectorGUI ()
	{
		Timer timer = target as Timer;
		EditorGUILayout.IntField("Timer Count", timer.timerHandleList.Count);


		if(GUILayout.Button("KillAllTimer"))
		{
			timer.KillAllTimerInternal(false);
		}
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("PauseAllTimer"))
		{
			timer.PauseAllTimerInternal();
		}
		if(GUILayout.Button("ResumeAllTimer"))
		{
			timer.ResumeAllTimerInternal();
		}
		EditorGUILayout.EndHorizontal();

		bFold = EditorGUILayout.Foldout(bFold, new GUIContent("Timer List", "Timer List"));
		if(bFold)
		{

			foreach(var th in timer.timerHandleList)
			{
				th.bShowDetail = EditorGUILayout.Foldout(th.bShowDetail, new GUIContent(getTimerString(th), ""), new GUIStyle(EditorStyles.foldout){richText = true});
				if(th.bShowDetail)
				{
					if(GUILayout.Button("Kill"))
					{
						th.Kill();
					}
					EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button("Pause"))
					{
						th.Pause();
					}
					if(GUILayout.Button("Resume"))
					{
						th.Resume();
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.FloatField("Delay", th.delay);
					th.interval = EditorGUILayout.FloatField("Interval", th.interval);
					if(th.maxCount == Timer.INFINITE)
					{
						EditorGUILayout.TextField("MaxCount", "Infinity");
					}
					else
					{
						th.maxCount = EditorGUILayout.IntField("MaxCount", th.maxCount);
					}
					EditorGUILayout.IntField("CountNow", th.countNow);

				}

			}
		}

	}
}
