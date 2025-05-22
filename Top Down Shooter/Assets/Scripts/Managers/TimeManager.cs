using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

	[SerializeField] private float resumeRate = 3;
	[SerializeField] private float pauseRate = 7;

	private float timeAdjustRate;
	private float targetTimeScale = 1;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			SlowMotion(1f);

		if (Mathf.Abs(Time.timeScale - targetTimeScale) > 0.05f)
		{
			float adjustRate = Time.unscaledDeltaTime * timeAdjustRate;
			Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, adjustRate);
		}
		else
			Time.timeScale = targetTimeScale;
	}

	public void PauseTime()
	{
		timeAdjustRate = pauseRate;
		targetTimeScale = 0;
	}

	public void ResumeTime()
	{
		timeAdjustRate = resumeRate;
		targetTimeScale = 1;
	}

	public void SlowMotion(float seconds) => StartCoroutine(SlowTimeCoroutine(seconds));

	private IEnumerator SlowTimeCoroutine(float seconds)
	{
		targetTimeScale = 0.5f;
		Time.timeScale = targetTimeScale;
		yield return new WaitForSecondsRealtime(seconds);
		ResumeTime();
	}
}
