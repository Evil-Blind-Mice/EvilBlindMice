using UnityEngine;
using static NESWDirections;
using static PossiblePaths;

public class ProceduralConnectionPoint : MonoBehaviour
{
    public Vector3 connectionPoint;

    //Enumuration for the 4 directions
    public ConnectionDirection direction;

    public LSRPaths choicePath;

    bool hasPassed = false;

    void Awake()
    {
        //Set variables on generation
        connectionPoint = transform.position;
        direction = GetDirection(transform.forward);
    }
    //Change hasPassed to true when collision is triggered
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hasPassed == false)
        {
            hasPassed = true;
            ProceduralGenerationManager.Instance.nextPathChoice = choicePath;
            ProceduralGenerationManager.Instance.nextCheckpointFlagged = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }

    //Calculate direction
    static ConnectionDirection GetDirection(Vector3 forwardVector)
    {
        float x = forwardVector.x;
        float z = forwardVector.z;

        if (x > 0)
        {
            return ConnectionDirection.North;
        }
        else if (x < 0)
        {
            return ConnectionDirection.South;
        }
        else if (z > 0)
        {
            return ConnectionDirection.West;
        }
        else
        {
            return ConnectionDirection.East;
        }
    }
}
