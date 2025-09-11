using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    // Variables for what directions are available

    [SerializeField] GameObject parent;
    public bool up, down, right, left, forward, backward;
    [HideInInspector] public Vector3[] availableDirections;

    private void Start()
    {
        availableDirections = new Vector3[6];
        if (up) availableDirections[0] = (parent.transform.up);
        if (down) availableDirections[1] = (-parent.transform.up);
        if (backward) availableDirections[2] = (parent.transform.right);
        if (forward) availableDirections[3] = (-parent.transform.right);
        if (left) availableDirections[4] = (parent.transform.forward);
        if (right) availableDirections[5] = (-parent.transform.forward);
    }

    public bool IsDirectionAvailable(Vector3 _direction)
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
