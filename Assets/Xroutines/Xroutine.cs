using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Xroutine : CustomYieldInstruction
{
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
            useStaticMonoBehaviour = m_monoBehaviour == null;
        }
    }

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            return isRunning;
        }
    }

    #region WaitFor aliases

    public Xroutine WaitFor(IEnumerator routine)
    {
        return WaitForRoutine(routine);
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
        return WaitForTask(action, threaded);
    }

    public Xroutine WaitFor(bool threaded = false, params Action[] actions)
    {
        return WaitForTasks(threaded, actions);
    }

    public Xroutine WaitFor(Animation animation)
    {
        return WaitForAnimation(animation);
    }

    public Xroutine WaitFor(AudioSource audioSource)
    {
        return WaitForAudioSource(audioSource);
    }

    public Xroutine WaitFor(Button button)
    {
        return WaitForButtonClick(button);
    }

    public Xroutine WaitFor(Collider collider)
    {
        return WaitForCollision(collider);
    }

    public Xroutine WaitFor(Collider2D collider2d)
    {
        return WaitForCollision2D(collider2d);
    }

    public Xroutine WaitFor(Thread thread)
    {
        return WaitForThread(thread);
    }

    public Xroutine WaitFor(UnityEvent unityEvent)
    {
        return WaitForEvent(unityEvent);
    }

    public Xroutine WaitFor(YieldInstruction instruction)
    {
        return WaitForYieldInstruction(instruction);
    }

    public Xroutine WaitFor(params YieldInstruction[] instructions)
    {
        for (int i = 0; i < instructions.Length; i++)
        {
            WaitFor(instructions[i]);
        }
        return this;
    }

    public Xroutine WaitFor(Xroutine xroutine)
    {
        return WaitForXroutine(xroutine);
    }

    public Xroutine WaitFor(params Xroutine[] xroutines)
    {
        for (int i = 0; i < xroutines.Length; i++)
        {
            WaitFor(xroutines[i]);
        }
        return this;
    }

    #endregion

    public static Xroutine Create(MonoBehaviour monoBehaviour = null)
    {
        return new Xroutine(monoBehaviour);
    }

    public static Xroutine Create(IEnumerator routine, MonoBehaviour monoBehaviour = null)
    {
        return new Xroutine(monoBehaviour).WaitFor(routine);
    }

    [Obsolete("Method Stop() has been deprecated. Use Abort() instead.")]
    public void Stop()
    {
        Abort();
    }

    public void Abort()
    {
        MonoBehaviour.StopCoroutine(coroutine);
        queue.Clear();
        isRunning = false;
        if (current.GetType() == typeof(WaitForTask))
        {
            ((WaitForTask)current).Stop();
        }
    }

    public Xroutine WaitForAnimation(Animation animation)
    {
        return WaitForRoutine(new WaitForAnimation(animation));
    }

    public Xroutine WaitForAudioSource(AudioSource audioSource)
    {
        return WaitForRoutine(new WaitForAudioSource(audioSource));
    }

    public Xroutine WaitForButtonClick(Button button)
    {
        return WaitForRoutine(new WaitForEvent(button.onClick));
    }

    public Xroutine WaitForCollision(Collider collider)
    {
        return WaitForRoutine(new WaitForCollision(collider));
    }

    public Xroutine WaitForCollision2D(Collider2D collider2d)
    {
        return WaitForRoutine(new WaitForCollision(collider2d));
    }

    public Xroutine WaitForEndOfFrame()
    {
        return WaitForYieldInstruction(new WaitForEndOfFrame());
    }

    public Xroutine WaitForFrames(int numberOfFrames)
    {
        for (int i = 0; i < numberOfFrames; i++)
        {
            WaitForEndOfFrame();
        }
        return this;
    }

    public Xroutine WaitForEvent(UnityEvent unityEvent)
    {
        return WaitForRoutine(new WaitForEvent(unityEvent));
    }

    public Xroutine WaitForFixedUpdate()
    {
        return WaitForYieldInstruction(new WaitForFixedUpdate());
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
        return WaitForYieldInstruction(new WaitForSeconds(seconds));
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

    public Xroutine WaitForThread(Thread thread)
    {
        return WaitForRoutine(new WaitForThread(thread));
    }

    public Xroutine WaitForXroutine(Xroutine xroutine)
    {
        if (xroutine == this)
            Debug.LogWarning("Xroutine added to its own queue: Xroutine will be waiting for itself and will never resolve.");
        return WaitForRoutine(xroutine);
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
        this.MonoBehaviour = monoBehaviour;
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
