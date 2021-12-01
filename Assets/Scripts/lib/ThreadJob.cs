using System.Collections;

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
            //bool tmp;
            //lock (handle)
            //{
            //    tmp = isDone;
            //}
            //return tmp;
            return isDone;
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
    private Node[] path;
    public bool pathObtained;

    public PathFindingJob()
    {
        pathObtained = false;
        pathFinder = new PathFinder();
    }

    protected override void ThreadFunction()
    {
        path = pathFinder.FindPath(startNode, endNode);
        IsDone = true;
    }
    protected override void OnFinished()
    {

    }

    public bool isPathNull()
    {
        return path == null;
    }
    public bool isPathZeroLength()
    {
        return path.Length <= 1;
    }

    public Node[] getPath()
    {
        pathObtained = true;

        if (path == null || path.Length == 0 || endNode == null || startNode == null)
        {
            return null;
        }

        //ensure there is a correct path
        var startNodeSuccessfullyCalculated = (startNode.location - path[0].location).magnitude <= WorldMapNodes.NODEDISTANCE * 2.0f;
        var endNodeSuccessfullyCalculated = (endNode.location - path[path.Length - 1].location).magnitude <= WorldMapNodes.NODEDISTANCE * 2.0f;

        if (startNodeSuccessfullyCalculated && endNodeSuccessfullyCalculated)
        {
            var temp = path;
            path = null;
            return temp;
        }

        return null;
    }

    public void SetEndFlag()
    {
        pathObtained = true;
        path = null;
        //Abort();
    }
}