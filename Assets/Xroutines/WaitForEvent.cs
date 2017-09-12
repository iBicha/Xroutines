using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitForEvent : CustomYieldInstruction
{
    private bool eventFired;
    private UnityEvent unityEvent;

    public WaitForEvent(UnityEvent unityEvent)
    {
        this.unityEvent = unityEvent;
        unityEvent.AddListener(FireEvent);
    }

    ~WaitForEvent()
    {
        CleanUp();
    }

    private void FireEvent()
    {
        eventFired = true;
        CleanUp();
    }

    private void CleanUp()
    {
        if (unityEvent != null)
        {
            unityEvent.RemoveListener(FireEvent);
            unityEvent = null;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            return !eventFired;
        }
    }
}
