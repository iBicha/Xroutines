# Xroutines
A mini framework to better use coroutines in Unity.
Easy, chainable actions , based on coroutines.

### Usage

With Xroutines you can:

 * Chain actions
 * You don't have to deal with `IEnumerator` or any `yield` instructions (If you don't want to, you still can)
 * You can use helper functions to wait for conditions quickly (`WaitForMouseDown`, `WaitForKeyDown`, etc)
 * You can add inline `Action` delegate methods 
 * Use `Stop()` to stop it

And here's some sample code:

```javascript
void Start () {
    Xroutine routine = Xroutine.Create()
        .Append(RoutineMethod1())
        .WaitForSeconds(0.25f)
        .Append(RoutineMethod2)
        .WaitForSeconds(0.25f)
        .Append(() => { Debug.Log("Execute code on the fly!"); });
}

//This is an enumerator method, that we can use with coroutines as usual
IEnumerator RoutineMethod1()
{
    Debug.Log("Routine Method 1");
    yield return new WaitForSeconds(0.1f);
    yield return null;
}
//this is just a method returning void, that we can also use with coroutines
void RoutineMethod2()
{
    Debug.Log("Routine Method 2");
}
```

You can check `Assets/Xroutines/Example` for more options.

### Contribution:

PRs and issues are always welcome.
If you think you have an idea for a `CustomYieldInstruction` that can be useful, please contribute, so we can have a library of most common yield instructions and have them built-in Xroutines
