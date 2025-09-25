using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float bobSpeed;
    [SerializeField] float rotateSpeed;

    Vector3 startLocalPosition;
    Vector3 startWorldPosition;
    Transform parent;
    float phase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = transform.parent;
        startLocalPosition = transform.localPosition;
        startWorldPosition = transform.position;
        phase = Random.Range(0, Mathf.PI * 2);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float newY = Mathf.Sin(Time.time * bobSpeed + phase) * amplitude;
        Vector3 anchor = parent ? parent.TransformPoint(startLocalPosition) : startWorldPosition;

        transform.position = anchor + transform.up * newY;
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
