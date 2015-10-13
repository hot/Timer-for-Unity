using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace KyUtils
{
	public delegate bool TimerFunc();

	[System.Serializable]
	public class TimerHandle
	{
		public bool bDead = false;
		public bool bPaused = false;

		public string tag;

		public TimerFunc func;
		public float delay;
		public float interval;
		public int maxCount;
		public int countNow;

		public Action completeCB;
		#if UNITY_EDITOR
		//for editor only
		public bool bShowDetail = false;
		#endif

		public TimerHandle OnComplete(Action cb)
		{
			completeCB = cb;
			return this;
		}

		public void Kill()
		{
			bDead = true;
			completeCB = null;
		}

		public void Pause()
		{
			if(IsValid())
			{
				bPaused = true;
			}
			else
			{
				Debug.LogError("Timer Already Dead!!");
			}
		}

		public void Resume()
		{
			if(IsValid())
			{
				bPaused = false;
			}
			else
			{
				Debug.LogError("Timer Already Dead!!");
			}
		}

		public bool IsValid()
		{
			return !bDead;
		}
	}

	public class Timer : MonoBehaviour {

		public const int INFINITE = -1;
		public string timerName;
		public bool bPaused = false;

		public LinkedList<TimerHandle> timerHandleList = new LinkedList<TimerHandle>();
		#if UNITY_EDITOR
		private void LateUpdate () {
			gameObject.name = "Timer_"+ timerName + "_" + timerHandleList.Count;
		}
		#endif


		#region internal world
		private static Timer _Instance = null;

		private IEnumerator _timer(TimerHandle th) {
			int countNow = 0;
			if(th.delay > 0f)
				yield return new WaitForSeconds(th.delay);
			while(th.maxCount != 0)
			{
				if(th.bDead == true)
					break;
				if(bPaused == true || th.bPaused == true)	
				{
					yield return 0;
				}
				else
				{
					if(th.maxCount != INFINITE)
					{
						++countNow;
					}
					#if UNITY_EDITOR
					++th.countNow;
					#endif
					if(th.func())	//If TimerFunc return true then end Timer Immediately.
						break;
					if(th.maxCount != INFINITE && countNow >= th.maxCount)
						break;
					yield return new WaitForSeconds(th.interval);
				}
			}
			//clean up timer
			th.bDead = true;
			if(th.completeCB != null)
			{
				th.completeCB();
			}
			#if UNITY_EDITOR
			timerHandleList.Remove(th);
			#endif
		}

		#endregion

		public static Timer CreateLocalTimer(string timerName)
		{
			GameObject go = new GameObject("Timer");
			Timer timer = go.AddComponent<Timer>();
			timer.timerName = timerName;
			return timer;
		}

		public TimerHandle addTimerInternal(TimerFunc func, float delay, float interval, int maxCount, string tag = "defaultTag")
		{
			TimerHandle th = new TimerHandle();
			th.func = func;
			th.delay = delay;
			th.interval = interval;
			th.maxCount = maxCount;
			th.tag = tag;

			StartCoroutine(_timer(th));
			#if UNITY_EDITOR
			th.countNow = 0;
			timerHandleList.AddLast(th);
			#endif
			return th;
		}

		/// <summary>
		/// Adds the timer to GLOBAL 
		/// </summary>
		/// <returns>The timer handle.</returns>
		/// <param name="func">Func.</param>
		/// <param name="delay">Delay.</param>
		/// <param name="interval">Interval.</param>
		/// <param name="maxCount">Max count.</param>
		/// <param name="tag">Tag, only for debug.</param>
		public static TimerHandle addTimerGlobal(TimerFunc func, float delay, float interval, int maxCount, string tag = "defaultTag")
		{
			if(_Instance == null)
			{
				GameObject go = new GameObject("Timer");
				DontDestroyOnLoad(go);
				_Instance = go.AddComponent<Timer>();
				_Instance.timerName = "Global";
			}
			TimerHandle th = _Instance.addTimerInternal(func, delay, interval, maxCount, tag);

			return th;
		}

		public void PauseAllTimerInternal()
		{
			bPaused = true;
		}
		public static void PauseAllTimerGlobal()
		{
			_Instance.PauseAllTimerInternal();
		}

		public void ResumeAllTimerInternal()
		{
			bPaused = false;
		}
		public static void ResumeAllTimerGlobal()
		{
			_Instance.ResumeAllTimerInternal();
		}

		public static void KillAllTimerGlobal(bool bDestroy)
		{
			_Instance.KillAllTimerInternal(bDestroy);
		}
		public void KillAllTimerInternal(bool bDestroy)
		{
			if(bDestroy)
			{
				Destroy(gameObject);
			}
			else
			{
				foreach(var th in timerHandleList)
				{
					th.bDead = true;
				}
			}

		}

	}
}
