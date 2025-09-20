using UnityEngine;

public class ChunkConnectionPoint : MonoBehaviour
{
    [SerializeField] float destroyDelay = 0.1f;
    [HideInInspector] public Quaternion rotation;
    [HideInInspector] public Vector3 position;
    [HideInInspector] public Vector3 forwardAxis;
    [HideInInspector] public Vector3 upAxis;
    [HideInInspector] public Vector3 rightAxis;
    [HideInInspector] public ChunkV2 child = null;

    void Awake()
    {
        rotation = transform.rotation;
        position = transform.position;
        forwardAxis = transform.forward;
        upAxis = transform.up;
        rightAxis = transform.right;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProceduralGenerationManagerV2.Instance.GenerateNextChunkSet(child, destroyDelay);
        }
    }

    public void SetChild(ChunkV2 _child)
    {
        child = _child;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        float scaleMuliplier = gameObject.transform.parent.transform.localScale.z;
        float increment = (15 * scaleMuliplier) + 2;
        for (int j = 0; j < 6; j++)
        {
            Gizmos.DrawSphere(transform.position + transform.forward * increment, 7.5f * scaleMuliplier);
            increment += (15 * scaleMuliplier);
        }
    }
}
