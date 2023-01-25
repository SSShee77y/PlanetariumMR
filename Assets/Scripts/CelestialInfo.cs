using UnityEngine;
using UnityEngine.Serialization;

public class CelestialInfo : MonoBehaviour
{
    
    [FormerlySerializedAs("_mass")][Tooltip("1 Unit = 1 Earth Mass")]
    public float mass = 1;

    [FormerlySerializedAs("_radius")][Tooltip("Visual purposes only")]
    public float radius = 1;

    [FormerlySerializedAs("_rotationSpeed")][Tooltip("1 Unit = 1 day")]
    public float rotationSpeed = 0;
    
    [FormerlySerializedAs("_initialSpeed")][Tooltip("1 Unit = 1,000,000 km / day")]
    public float initialSpeed = 10;

    [FormerlySerializedAs("_semiMajorAxis")][Tooltip("1 Unit = 1,000,000 km | Basically Average Distance from Sun")]
    public float semiMajorAxis = 0;

    [FormerlySerializedAs("_orbitalPrimary")][Tooltip("Main planet it orbits (useful if using specific orbit type")]
    public GameObject orbitalPrimary = null;
    
    public enum OrbitType
    {
        None,
        Perfect,
        Elliptic
    }

    [FormerlySerializedAs("_setToOrbit")]
    public OrbitType setToOrbit = OrbitType.None;

    [Tooltip("Relative to earth's elliptic plane")] [FormerlySerializedAs("_axialTilt")]
    public float axialTilt = 0f;

    [Tooltip("Relative to earth's elliptic plane")] [FormerlySerializedAs("_inclination")]
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
