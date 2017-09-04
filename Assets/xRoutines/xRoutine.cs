using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xroutine {
    private static MonoBehaviour s_monoBehaviour;
    private MonoBehaviour m_monoBehaviour;
    private bool useStaticMonoBehaviour = true;
	private Queue<IEnumerator> queue;
	private IEnumerator current;
    private bool isRunning = false;
    private Coroutine coroutine;

    public MonoBehaviour MonoBehaviour
    { 
        get
        {
            if (useStaticMonoBehaviour)
            {
                if (s_monoBehaviour == null)
                {
                    GameObject go = new GameObject("{Xroutine}");
                    UnityEngine.Object.DontDestroyOnLoad(go);
                    s_monoBehaviour = go.AddComponent<DummyMonoBehaviour>();
                    Debug.Log("{Xroutine} singleton created.");
                }
                return s_monoBehaviour;

            }
            return m_monoBehaviour;
        }
        set
        {
            m_monoBehaviour = value;
            useStaticMonoBehaviour = false;
        }
    }

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
    }

	#region WaitFor aliases

	public Xroutine WaitFor(IEnumerator routine)
	{
		return WaitForRoutine (routine);
	}

	public Xroutine WaitFor(params IEnumerator[] routines)
	{
		for (int i = 0; i < routines.Length; i++)
		{
			WaitFor(routines[i]);
		}
		return this;
	}

	public Xroutine WaitFor(Action action, bool threaded = false)
	{
		return WaitForTask (action, threaded);
	}

	public Xroutine WaitFor(bool threaded = false, params Action[] actions)
	{
		return WaitForTasks (threaded, actions);
	}

	public Xroutine WaitFor(Animation animation)
	{
		return WaitForAnimation(animation);
	}

	public Xroutine WaitFor(AudioSource audioSource)
	{
		return WaitForAudioSource(audioSource);
	}

	public Xroutine WaitFor(YieldInstruction instruction)
	{
		return WaitForYieldInstruction (instruction);
	}

	public Xroutine WaitFor(params YieldInstruction[] instructions)
	{
		for (int i = 0; i < instructions.Length; i++)
		{
			WaitFor (instructions[i]);
		}
		return this;
	}

	public Xroutine WaitFor(Xroutine xroutine)
	{
		return WaitForXroutine (xroutine);
	}

	public Xroutine WaitFor(params Xroutine[] xroutines)
	{
		for (int i = 0; i < xroutines.Length; i++)
		{
			WaitFor (xroutines[i]);
		}
		return this;
	}

	#endregion

    public static Xroutine Create(MonoBehaviour monoBehaviour = null)
    {
        return new Xroutine(monoBehaviour);
    }

	public void Stop()
	{
		MonoBehaviour.StopCoroutine(coroutine);
		queue.Clear();
		isRunning = false;
		if (current.GetType () == typeof(WaitForTask)) {
			((WaitForTask)current).Stop ();
		}
	}

	public Xroutine WaitForAnimation(Animation animation)
	{
		return WaitForRoutine (new WaitForAnimation (animation));
	}

	public Xroutine WaitForAudioSource(AudioSource audioSource)
	{
		return WaitForRoutine (new WaitForAudioSource (audioSource));
	}

	public Xroutine WaitForEndOfFrame()
	{
		return WaitForYieldInstruction (new WaitForEndOfFrame ());
	}

	public Xroutine WaitForFixedUpdate()
	{
		return WaitForYieldInstruction (new WaitForFixedUpdate ());
	}
		
	public Xroutine WaitForKeyDown(string name)
	{
		return WaitForRoutine(new WaitForKeyDown(name));
	}

	public Xroutine WaitForKeyDown(KeyCode key)
	{
		return WaitForRoutine(new WaitForKeyDown(key));
	}

	public Xroutine WaitForMouseDown(int button)
	{
		return WaitForRoutine(new WaitForMouseDown(button));
	}

	public Xroutine WaitForRoutine(IEnumerator routine)
	{
		queue.Enqueue(routine);
		if (!isRunning)
		{
			coroutine = MonoBehaviour.StartCoroutine(routineMain());
			isRunning = true;
		}
		return this;
	}

	public Xroutine WaitForSeconds(float seconds)
	{
		return WaitForYieldInstruction (new WaitForSeconds (seconds));
	}

	public Xroutine WaitForSecondsRealtime(float seconds)
	{
		return WaitForRoutine(new WaitForSecondsRealtime(seconds));
	}

	public Xroutine WaitForTask(Action action, bool threaded = false)
	{
		return WaitForRoutine(new WaitForTask(threaded, action));
	}

	public Xroutine WaitForTasks(bool threaded = false, params Action[] actions)
	{
		return WaitForRoutine(new WaitForTask(threaded, actions));
	}

	public Xroutine WaitForXroutine(Xroutine xroutine)
	{
		return WaitForRoutine(new WaitForXroutine(xroutine));
	}

	public Xroutine WaitForYieldInstruction(YieldInstruction yieldInstruction)
	{
		return WaitForRoutine(routineYieldInstruction(yieldInstruction));
	}

	public Xroutine WaitUntil(Func<bool> predicate)
	{
		return WaitForRoutine(new WaitUntil(predicate));
	}

	public Xroutine WaitWhile(Func<bool> predicate)
	{
		return WaitForRoutine(new WaitWhile(predicate));
	}

	private Xroutine(MonoBehaviour monoBehaviour)
    {
        if(monoBehaviour != null)
        {
            this.m_monoBehaviour = monoBehaviour;
            useStaticMonoBehaviour = false;
        }
        queue = new Queue<IEnumerator>();
    }
		
    private IEnumerator routineYieldInstruction(YieldInstruction yieldInstruction)
    {
        yield return yieldInstruction;
    }

    private IEnumerator routineMain()
    {
        while (queue.Count > 0)
        {
			current = queue.Dequeue();
			yield return current;
        }
        isRunning = false;
    }

    private class DummyMonoBehaviour : MonoBehaviour
    {

    }
}
