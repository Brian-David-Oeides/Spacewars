using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUtility : MonoBehaviour 
{
    private static TimerUtility _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the timer persists across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    public static void InvokeAfterTime(float time, System.Action action)
    {
        if (_instance == null)
        {
            GameObject timerObject = new GameObject("TimerUtility");
            _instance = timerObject.AddComponent<TimerUtility>();
        }

        _instance.StartCoroutine(_instance.InvokeRoutine(time, action));
    }

    private IEnumerator InvokeRoutine(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }
}
