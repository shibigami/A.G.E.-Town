using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadJob
{
    private bool isDone = false;
    private object handle = new object();
    private System.Threading.Thread thread = null;
    public bool IsDone
    {
        get
        {
            //create a lock to access isDone
            bool tmp;
            lock (handle)
            {
                tmp = isDone;
            }
            return tmp;
        }
        set
        {
            //create lock to set isDone
            lock (handle)
            {
                isDone = value;
            }
        }
    }

    //start the thread
    public virtual void Start()
    {
        thread = new System.Threading.Thread(Run);
        thread.Start();
    }
    //abort process
    public virtual void Abort()
    {
        thread.Abort();
    }

    //virtual method for function to run on thread
    protected virtual void ThreadFunction() { }

    //virtual method for any necessary logic once the job is done
    protected virtual void OnFinished() { }

    //update to be called when necessary and run logic once the job is done, this will also return true if the job is complete
    public virtual bool Update()
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }

    //Method for Unity's coroutine implementation
    public IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }

    //run the job
    private void Run()
    {
        ThreadFunction();
        IsDone = true;
    }
}

//pathfinding thread job
public class PathFindingJob : ThreadJob
{
    //nodes for use in job
    public Node startNode; 
    public Node endNode;
    //pathfinder class for path calculations
    private PathFinder pathFinder;
    //result
    public Node[] path; 

    protected override void ThreadFunction()
    {
        if (pathFinder == null) pathFinder = new PathFinder();
        path = pathFinder.FindPath(startNode, endNode);
    }
    protected override void OnFinished()
    {
        
    }
}