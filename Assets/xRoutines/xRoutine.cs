using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xroutine {
    private static MonoBehaviour s_monoBehaviour;
    private MonoBehaviour m_monoBehaviour;
    private bool useStaticMonoBehaviour = true;
    private Queue<IEnumerator> queue;
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

    public static Xroutine Create(MonoBehaviour monoBehaviour = null)
    {
        return new Xroutine(monoBehaviour);
    }

    public void Stop()
    {
        MonoBehaviour.StopCoroutine(coroutine);
        queue.Clear();
        isRunning = false;
    }

    public Xroutine Append(params IEnumerator[] routines)
    {
        for (int i = 0; i < routines.Length; i++)
        {
            Append(routines[i]);
        }
        return this;
    }

    public Xroutine Append(IEnumerator routine)
    {
        queue.Enqueue(routine);
        if (!isRunning)
        {
            coroutine = MonoBehaviour.StartCoroutine(routineMain());
            isRunning = true;
        }
        return this;
    }


    public Xroutine Append(YieldInstruction yieldInstruction)
    {
        Append(routineYieldInstruction(yieldInstruction));
        return this;
    }

    public Xroutine Append(params Action[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            Append(actions[i]);
        }
        return this;
    }

    public Xroutine Append(Action action)
    {
        Append(routineAction(action));
        return this;
    }

    public Xroutine WaitForSeconds(float seconds)
    {
        Append(routineYieldInstruction(new WaitForSeconds(seconds)));
        return this;
    }
    public Xroutine WaitForSecondsRealtime(float seconds)
    {
        Append(new WaitForSecondsRealtime(seconds));
        return this;
    }

    public Xroutine WaitForEndOfFrame()
    {
        Append(routineYieldInstruction(new WaitForEndOfFrame()));
        return this;
    }

    public Xroutine WaitForFixedUpdate()
    {
        Append(routineYieldInstruction(new WaitForFixedUpdate()));
        return this;
    }

    public Xroutine WaitWhile(Func<bool> predicate)
    {
        Append(new WaitWhile(predicate));
        return this;
    }

    public Xroutine WaitUntil(Func<bool> predicate)
    {
        Append(new WaitUntil(predicate));
        return this;
    }

    public Xroutine WaitForKeyDown(string name)
    {
        Append(new WaitForKeyDown(name));
        return this;
    }

    public Xroutine WaitForKeyDown(KeyCode key)
    {
        Append(new WaitForKeyDown(key));
        return this;
    }

    public Xroutine WaitForMouseDown(int button)
    {
        Append(new WaitForMouseDown(button));
        return this;
    }
		
	public Xroutine WaitForXroutine(Xroutine routine)
	{
		Append(new WaitForXroutine(routine));
		return this;
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

    private IEnumerator routineAction(Action action)
    {
        action.Invoke();
        yield return null;
    }

    private IEnumerator routineYieldInstruction(YieldInstruction yieldInstruction)
    {
        yield return yieldInstruction;
    }

    private IEnumerator routineMain()
    {
        while (queue.Count > 0)
        {
            IEnumerator routine = queue.Dequeue();
            yield return routine;
        }
        isRunning = false;
    }

    private class DummyMonoBehaviour : MonoBehaviour
    {

    }
}
