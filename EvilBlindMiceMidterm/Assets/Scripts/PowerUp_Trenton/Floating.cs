using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float bobSpeed;
    [SerializeField] float rotateSpeed;

    Vector3 startLocalPosition;
    Transform parent;
    float phase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = transform.parent;
        startLocalPosition = transform.localPosition;
        phase = Random.Range(0, Mathf.PI * 2);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float newY = Mathf.Sin(Time.time * bobSpeed + phase) * amplitude;
        Vector3 upInParentSpace = parent ? parent.InverseTransformDirection(transform.up) : transform.up;

        transform.localPosition = startLocalPosition + upInParentSpace * newY;
        transform.Rotate(transform.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
