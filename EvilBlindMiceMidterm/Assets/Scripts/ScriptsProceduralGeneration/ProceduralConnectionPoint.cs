using System.Timers;
using UnityEngine;
using static NESWDirections;
using static PossiblePaths;

public class ProceduralConnectionPoint : MonoBehaviour
{
    public Vector3 connectionPoint;

    //Enumuration for the 4 directions
    public FacingDirection direction;

    public LSRPaths choicePath;

    bool hasPassed = false;

    bool activateTimer1 = false;
    bool activateTimer2 = false;

    float timer1 = 0.0f;
    float timer2 = 0.0f;

    public float timeToWait = 1.0f;



    void Awake()
    {
        //Set variables on generation
        connectionPoint = transform.position;
        direction = GetDirection(transform.forward);
    }

    void Update()
    {
        if (activateTimer1)
        {
            timer1 += Time.deltaTime;
        }

        if (activateTimer2)
        {
            timer2 += Time.deltaTime;
        }

        if (timer1 >= timeToWait)
        {
            activateTimer1 = false;
            timer1 = 0.0f;
            ProceduralGenerationManager.Instance.nextPathChoice = choicePath;
            ProceduralGenerationManager.Instance.nextCheckpointFlagged = true;
        }

        if (timer2 >= timeToWait)
        {
            activateTimer2 = false;
            timer2 = 0.0f;
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }

    //Change hasPassed to true when collision is triggered
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hasPassed == false)
        {
            hasPassed = true;
            activateTimer1 = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activateTimer2 = true;
        }
    }

    //Calculate direction
    static FacingDirection GetDirection(Vector3 forwardVector)
    {
        float absoluteX = Mathf.Abs(forwardVector.x);
        float absoluteZ = Mathf.Abs(forwardVector.z);

        if (absoluteX > absoluteZ)
        {
            if (forwardVector.x > 0)
            {
                return FacingDirection.North;
            }
            else
            {
                return FacingDirection.South;
            }
        }
        else
        {
            if (forwardVector.z > 0)
            {
                return FacingDirection.West;
            }
            else
            {
                return FacingDirection.East;
            }
        }
    }
}
