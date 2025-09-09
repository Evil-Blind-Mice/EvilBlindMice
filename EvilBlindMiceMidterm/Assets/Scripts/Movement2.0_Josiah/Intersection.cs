using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    // Variables for what directions are available

    public bool up, down, right, left, forward, backward;
    [HideInInspector] public Vector3[] availableDirections;

    private void Start()
    {
        availableDirections = new Vector3[6];
        if (up) availableDirections[0] = (transform.up);
        if (down) availableDirections[1] = (-transform.up);
        if (right) availableDirections[2] = (transform.right);
        if (left) availableDirections[3] = (-transform.right);
        if (forward) availableDirections[4] = (transform.forward);
        if (backward) availableDirections[5] = (-transform.forward);
    }

    public bool DirectionAvailable(Vector3 _direction)
    {
        foreach (Vector3 nextVec in availableDirections)
        {
            if (nextVec == _direction)
            {
                return true;
            }
        }
        return false;
    }
}
