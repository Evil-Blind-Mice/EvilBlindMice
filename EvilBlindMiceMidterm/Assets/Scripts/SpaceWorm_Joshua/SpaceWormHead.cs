using UnityEngine;

public class SpaceWormHead : MonoBehaviour
{
    public float swayAmplitude = 15f; // degrees
    public float swayFrequency = 1f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swayFrequency) * swayAmplitude;
        transform.localRotation = startRotation * Quaternion.Euler(0, sway, 0);
    }
}