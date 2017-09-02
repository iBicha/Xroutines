using UnityEngine;

public class WaitForXRoutine : CustomYieldInstruction
{
    private Xroutine routine;

	public WaitForXRoutine(Xroutine routine)
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
