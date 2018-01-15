using System.Collections;
using UnityEngine;

public class XroutineTest : MonoBehaviour {

	void Start () {
        //create a routine, and chain actions on it
		Xroutine routine = Xroutine.Create(this)
			.WaitFor(RoutineMethod1())
            .WaitForSeconds(0.25f)
			.WaitFor(RoutineMethod2)
            .WaitForSeconds(0.25f);
        //You can decide to add stuff later.
        routine.WaitForSecondsRealtime(0.25f)
            //You can also add multiple methods to execute. They will still run one after the other
			.WaitFor(RoutineMethod1(), RoutineMethod1())
			.WaitFor(false, RoutineMethod2, RoutineMethod2)
            .WaitForFixedUpdate()
            //You can also add yield instructions like this
			.WaitFor(new WaitForSecondsRealtime(0.2f))
			.WaitFor(new WaitForSeconds(0.3f))
			.WaitFor(() => { Debug.Log("Execute code on the fly!"); })
			.WaitFor(() => { Debug.Log("Press Enter to continue..."); })
            .WaitForKeyDown(KeyCode.Return)
            .WaitFor(() => { Debug.Log("Thanks!"); })
            //Waiting for 5 frames
            .WaitForFrames(5)
            .WaitFor(() => { Debug.Log("Left click to continue..."); })
            .WaitForMouseDown(0)
			.WaitFor(() => { Debug.Log("Thanks again!"); })
			.WaitFor(() => { 
				Debug.Log("Sleeping for 3 seconds on background thread..."); 
				System.Threading.Thread.Sleep(3000); 
				Debug.Log("Done Sleeping"); 
			}, true)
			.WaitFor(() => { Debug.Log(string.Format("Xroutine is still Running: {0}", routine.IsRunning)); })
			.WaitFor(() => { Debug.Log("We are going to stop now."); })
            //I will stop it here
            //I can call Abort() from anywhere else for immediate interruption.
			.WaitFor(() => { routine.Abort(); })
			.WaitFor(() => { Debug.Log("This will not be executed"); });

        //We can even start another routine, and wait for the previous one to finish.
		Xroutine.Create()
            .WaitForXroutine(routine)
			.WaitFor(() => { Debug.Log("Routine just finished executing."); })
			.WaitFor(() => { Debug.Log(string.Format("Xroutine is still Running: {0}", routine.IsRunning)); });

        //Note: if you call WaitForXRoutine on itself, it will be running a task of checking if that task is done. Which will never be.
        //In other words, calling WaitForXRoutine on itself blocks the routine, until Stop() is called somewhere else.
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
}
