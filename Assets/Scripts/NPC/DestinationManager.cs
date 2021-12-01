using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationManager : MonoBehaviour
{
    private Citizen[] citizens;
    private PathFindingJob[,] pathFindingJob;
    private bool[,] isCalculatingPath;
    private Node[] moveToPoints;
    private int currentTaskCitizenIndex;
    private bool pathProcessingCoroutineRunning;
    public float pathProcessingRecallTime;


    // Start is called before the first frame update
    void Start()
    {
        //make sure game manager initializes before assigning citizens to array
        Constants.gameManager.Start();

        //assign citizens to array
        List<Citizen> tempCitizenList = new List<Citizen>();
        foreach (var cit in Constants.gameManager.citizens)
        {
            tempCitizenList.Add(cit.GetComponent<Citizen>());
        }
        citizens = tempCitizenList.ToArray();

        moveToPoints = new Node[citizens.Length];
        currentTaskCitizenIndex = 0;

        //pathfinder
        pathFindingJob = new PathFindingJob[citizens.Length, Constants.THREADSPERCITIZEN];
        isCalculatingPath = new bool[citizens.Length, Constants.THREADSPERCITIZEN];
        for (int denizen = 0; denizen < isCalculatingPath.GetLength(0); denizen++)
        {
            for (int pathJob = 0; pathJob < isCalculatingPath.GetLength(1); pathJob++)
            {
                isCalculatingPath[denizen, pathJob] = false;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //move on to path processing handling if coroutine is not running
        if (!pathProcessingCoroutineRunning && citizens.Length > 0)
        {
            //go to next citizen in queue
            if (currentTaskCitizenIndex + 1 >= citizens.Length)
            {
                currentTaskCitizenIndex = 0;
            }
            else
            {
                currentTaskCitizenIndex++;
            }

            //needs to have a path calculated
            if (citizens[currentTaskCitizenIndex].NeedsPathing() || citizens[currentTaskCitizenIndex].NeedsSocialMovePoint())
            {
                StartCoroutine("MovePointsHandling");
            }
        }
    }


    private IEnumerator FindPath(int citizen, int jobIndex)
    {
        //check if finished
        yield return StartCoroutine(pathFindingJob[citizen, jobIndex].WaitFor());
    }

    private IEnumerator MovePointsHandling()
    {
        pathProcessingCoroutineRunning = true;

        if (!WorldMapNodes.Instance.isMapComplete())
        {
            yield return new WaitUntil(WorldMapNodes.Instance.isMapComplete);
        }

        for (int i = 0; i < isCalculatingPath.GetLength(1); i++)
        {
            if (!isCalculatingPath[currentTaskCitizenIndex, i])
            {
                StartCoroutine("CalculatePath", i);
                yield return new WaitForEndOfFrame();
                continue;
            }

            ////if there is a path already calculated
            if (pathFindingJob[currentTaskCitizenIndex, i].IsDone)
            {
                moveToPoints = pathFindingJob[currentTaskCitizenIndex, i].getPath();

                if (moveToPoints != null)
                {
                    //reset calculating flags
                    //isCalculatingPath = new bool[Constants.THREADSPERCITIZEN];

                    //set end flag for remaining threads
                    for (int c = 0; c < pathFindingJob.GetLength(1); c++)
                    {
                        if (pathFindingJob[currentTaskCitizenIndex, c] != null)
                        {
                            pathFindingJob[currentTaskCitizenIndex, c].SetEndFlag();
                        }
                        isCalculatingPath[currentTaskCitizenIndex, c] = false;
                    }

                    //send path to citizen
                    citizens[currentTaskCitizenIndex].SetPath(moveToPoints);

                    break;
                }
                else
                {
                    isCalculatingPath[currentTaskCitizenIndex, i] = false;
                }
            }
        }

        yield return new WaitForSecondsRealtime(pathProcessingRecallTime);

        pathProcessingCoroutineRunning = false;
    }

    //call coroutine and await for each unsuccessful path finding operation needed
    private IEnumerator CalculatePath(int index)
    {
        if (!isCalculatingPath[currentTaskCitizenIndex, index])
        {
            if (pathFindingJob[currentTaskCitizenIndex, index] == null)
            {
                pathFindingJob[currentTaskCitizenIndex, index] = Constants.gameManager.PathFindingJobsPool;
            }

            if (pathFindingJob[currentTaskCitizenIndex, index].isPathNull() ||
                pathFindingJob[currentTaskCitizenIndex, index].isPathZeroLength() ||
                pathFindingJob[currentTaskCitizenIndex, index].pathObtained)
            {
                if (citizens[currentTaskCitizenIndex].NeedsSocialMovePoint())
                {
                    citizens[currentTaskCitizenIndex].SocialMovePointObtained();
                    var targetCitizen = Constants.gameManager.GetNextAvailableCitizenForSocializing(citizens[currentTaskCitizenIndex]);
                    if (targetCitizen != null)
                    {
                        citizens[currentTaskCitizenIndex].SetLookAtPoint(targetCitizen.transform.position);
                        yield return new WaitForEndOfFrame();
                        if (targetCitizen.socialPoint != null)
                        {
                            pathFindingJob[currentTaskCitizenIndex, index].endNode = targetCitizen.socialPoint;
                        }
                        else
                        {
                            pathFindingJob[currentTaskCitizenIndex, index].endNode = WorldMapNodes.Instance.getAvailableNodeBetween(
                                        new Node(transform.position.x, transform.position.z),
                                        new Node(targetCitizen.transform.position.x, targetCitizen.transform.position.z)
                                        );
                            if (pathFindingJob[currentTaskCitizenIndex, index].endNode != null)
                            {
                                targetCitizen.SetSocialPoint(pathFindingJob[currentTaskCitizenIndex, index].endNode);
                            }
                        }
                    }
                }
                else if (citizens[currentTaskCitizenIndex].NeedsPathing())
                {
                    //decide on end node based on current need or schedule
                    pathFindingJob[currentTaskCitizenIndex, index].endNode = citizens[currentTaskCitizenIndex].GetNodeBasedOnAction();
                }

                if (pathFindingJob[currentTaskCitizenIndex, index].endNode != null)
                {
                    pathFindingJob[currentTaskCitizenIndex, index].startNode = citizens[currentTaskCitizenIndex].GetCurrentNode();
                    //run thread
                    pathFindingJob[currentTaskCitizenIndex, index].Start();
                    isCalculatingPath[currentTaskCitizenIndex, index] = true;
                }
            }
        }

        yield return new WaitForEndOfFrame();
    }

    private void OnValidate()
    {
        pathProcessingRecallTime = Mathf.Clamp(pathProcessingRecallTime, 0.01f, 1f);
    }
}
