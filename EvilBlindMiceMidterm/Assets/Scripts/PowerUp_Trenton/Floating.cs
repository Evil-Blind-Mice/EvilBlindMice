using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float bobSpeed;
    [SerializeField] float rotateSpeed;

    Vector3 startLocalPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float newY = Mathf.Sin(Time.time * bobSpeed) * amplitude;
        transform.localPosition = startLocalPosition + Vector3.up * newY;
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
