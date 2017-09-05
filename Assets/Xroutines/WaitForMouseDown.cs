using UnityEngine;

public class WaitForMouseDown : CustomYieldInstruction
{
    private int button;

    public WaitForMouseDown(int button)
    {
        this.button = button;
    }

    public override bool keepWaiting
    {
        get
        {
            return !Input.GetMouseButtonDown(button);
        }
    }

}
