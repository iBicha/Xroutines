using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class WaitForThread : CustomYieldInstruction
{
    private Thread thread;

    public WaitForThread(Thread thread)
    {
        this.thread = thread;
    }

    public override bool keepWaiting
    {
        get
        {
            return thread.IsAlive;
        }
    }
}
