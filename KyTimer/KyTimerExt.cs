using UnityEngine;
using System.Collections;
using KyUtils;

namespace KyExt
{
	public static class TimerExtForMonoBehaviour
	{
		private static IEnumerator _timer(TimerHandle th) {
			int countNow = 0;
			if(th.delay > 0f)
				yield return new WaitForSeconds(th.delay);
			while(th.maxCount != 0)
			{
				if(th.bDead == true)
					break;
				if(th.bPaused == true)	
				{
					yield return 0;
				}
				else
				{
					if(th.maxCount != Timer.INFINITE)
					{
						++countNow;
					}
					#if UNITY_EDITOR
					++th.countNow;
					#endif
					if(th.func())	//If TimerFunc return true then end Timer Immediately.
						break;
					if(th.maxCount != Timer.INFINITE && countNow >= th.maxCount)
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
		}

		public static TimerHandle AddTimer(this MonoBehaviour mb, TimerFunc func, float delay, float interval, int maxCount, string tag = "defaultTag")
		{
			TimerHandle th = new TimerHandle();
			th.func = func;
			th.delay = delay;
			th.interval = interval;
			th.maxCount = maxCount;
			th.tag = tag;
			
			mb.StartCoroutine(_timer(th));
			return th;
		}
	}   
}