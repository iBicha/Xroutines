using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xRoutine {
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
                    GameObject go = new GameObject("{xRoutine}");
                    UnityEngine.Object.DontDestroyOnLoad(go);
                    s_monoBehaviour = go.AddComponent<DummyMonoBehaviour>();
                    Debug.Log("{xRoutine} singleton created.");
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

    public static xRoutine Create(MonoBehaviour monoBehaviour = null)
    {
        return new xRoutine(monoBehaviour);
    }

    public void Stop()
    {
        MonoBehaviour.StopCoroutine(coroutine);
        queue.Clear();
        isRunning = false;
    }

    public xRoutine Append(params IEnumerator[] routines)
    {
        for (int i = 0; i < routines.Length; i++)
        {
            Append(routines[i]);
        }
        return this;
    }

    public xRoutine Append(IEnumerator routine)
    {
        queue.Enqueue(routine);
        if (!isRunning)
        {
            coroutine = MonoBehaviour.StartCoroutine(routineMain());
            isRunning = true;
        }
        return this;
    }


    public xRoutine Append(YieldInstruction yieldInstruction)
    {
        Append(routineYieldInstruction(yieldInstruction));
        return this;
    }

    public xRoutine Append(params Action[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            Append(actions[i]);
        }
        return this;
    }

    public xRoutine Append(Action action)
    {
        Append(routineAction(action));
        return this;
    }

    public xRoutine WaitForSeconds(float seconds)
    {
        Append(routineYieldInstruction(new WaitForSeconds(seconds)));
        return this;
    }
    public xRoutine WaitForSecondsRealtime(float seconds)
    {
        Append(new WaitForSecondsRealtime(seconds));
        return this;
    }

    public xRoutine WaitForEndOfFrame()
    {
        Append(routineYieldInstruction(new WaitForEndOfFrame()));
        return this;
    }

    public xRoutine WaitForFixedUpdate()
    {
        Append(routineYieldInstruction(new WaitForFixedUpdate()));
        return this;
    }

    public xRoutine WaitWhile(Func<bool> predicate)
    {
        Append(new WaitWhile(predicate));
        return this;
    }

    public xRoutine WaitUntil(Func<bool> predicate)
    {
        Append(new WaitUntil(predicate));
        return this;
    }

    public xRoutine WaitForKeyDown(string name)
    {
        Append(new WaitForKeyDown(name));
        return this;
    }

    public xRoutine WaitForKeyDown(KeyCode key)
    {
        Append(new WaitForKeyDown(key));
        return this;
    }

    public xRoutine WaitForMouseDown(int button)
    {
        Append(new WaitForMouseDown(button));
        return this;
    }

    public xRoutine WaitForXRoutine(xRoutine routine)
    {
        Append(new WaitForXRoutine(routine));
        return this;
    }

    private xRoutine(MonoBehaviour monoBehaviour)
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
