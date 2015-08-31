using UnityEngine;
using System.Collections;
using KyUtils;

public class TestTimer : MonoBehaviour {

	private TimerHandle th;
	// Use this for initialization
	void Start () {
		Debug.Log("start :" + Time.time);
		th = Timer.addTimerGlobal(()=>{
			Debug.Log("timeNow:" + Time.time);
			return false;
		}, 1f, 1f, Timer.INFINITE, "firstTimer");

		Timer.addTimerGlobal(()=>{
			Debug.Log("SecondSecond:" + Time.time);
			return false;
		}, 1f, 1f, 30, "Second");




	}

	public void OnClick()
	{
		th.bPaused = !th.bPaused;
	}


}
