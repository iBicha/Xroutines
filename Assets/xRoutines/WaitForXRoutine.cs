using UnityEngine;

public class WaitForXRoutine : CustomYieldInstruction
{
    private xRoutine routine;

    public WaitForXRoutine(xRoutine routine)
    {
        this.routine = routine;
    }

    public override bool keepWaiting
    {
        get
        {
            return routine.IsRunning;
        }
    }

}
