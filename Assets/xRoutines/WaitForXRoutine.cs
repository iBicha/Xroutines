using UnityEngine;

public class WaitForXroutine : CustomYieldInstruction
{
    private Xroutine routine;

	public WaitForXroutine(Xroutine routine)
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
