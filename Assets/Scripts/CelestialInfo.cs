using UnityEngine;
using UnityEngine.Serialization;

public class CelestialInfo : MonoBehaviour
{
    
    [Tooltip("1 Unit = 1 Earth Mass")]
    public float mass = 1f;

    [Tooltip("Visual purposes only so far | 1 Unit = 1,000,000 km")]
    public float radius = 1f;

    [Tooltip("1 Unit = 1 day")]
    public float rotationSpeed = 0f;
    
    [Tooltip("1 Unit = 1,000,000 km / day")]
    public float initialSpeed = 0f;

    [Tooltip("1 Unit = 1,000,000 km | Basically Average Distance from Sun")]
    public float semiMajorAxis = 0f;

    [Tooltip("Main planet it orbits (useful if using specific orbit type")]
    public GameObject orbitalPrimary = null;
    
    public enum OrbitType
    {
        None,
        Perfect,
        Elliptic
    }

    public OrbitType setToOrbit = OrbitType.None;

    [Tooltip("Relative to earth's elliptic plane")]
    public float axialTilt = 0f;

    [Tooltip("Relative to earth's elliptic plane")]
    public float inclination = 0.0f;

    private float _parentObjectScale = 1f;
    private float _defaultObjectScale = 1f;

    void Awake()
    {
        UpdateScales();
        GetComponent<Rigidbody>().mass = mass;
        transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        transform.eulerAngles = new Vector3(axialTilt, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void RotateOnAxis()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotationSpeed / 50, transform.eulerAngles.z);
    }

    public OrbitType GetOrbitType()
    {
        return setToOrbit;
    }

    public bool DoScalesMatch()
    {
        return _parentObjectScale.Equals(transform.parent.localScale.x);
    }

    public void UpdateScales()
    {
        _parentObjectScale = transform.parent.localScale.x;
    }

    public float GetPreviousScale()
    {
        return _parentObjectScale;
    }

    public float GetDefaultScale()
    {
        return _defaultObjectScale;
    }
}
