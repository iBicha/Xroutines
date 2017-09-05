using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForKeyDown : CustomYieldInstruction
{
    private bool useName;
    private string name;
    private KeyCode key;

    public WaitForKeyDown(string name)
    {
        this.name = name;
        useName = true;
    }

    public WaitForKeyDown(KeyCode key)
    {
        this.key = key;
        useName = false;
    }

    public override bool keepWaiting
    {
        get
        {
            return !(useName ? Input.GetKeyDown(name) : Input.GetKeyDown(key));
        }
    }
}
