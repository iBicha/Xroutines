using UnityEngine;

public class WaitForXroutine : CustomYieldInstruction
{
    private Xroutine xroutine;

	public WaitForXroutine(Xroutine xroutine)
    {
        this.xroutine = xroutine;
    }

    public override bool keepWaiting
    {
        get
        {
            return xroutine.IsRunning;
        }
    }

}
