using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
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

    [Tooltip("1 Unit = 1,000,000 km | The longest distance from its orbital primary")]
    public float apocenter = 0f;

    [Tooltip("1 Unit = 1,000,000 km | The shortest distance from its orbital primary")]
    public float pericenter = 0f;

    [Tooltip("1 Unit = 1,000,000 km | Essentially the average distance from its orbital primary")]
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

    private float previousParentScale = 1f;

    private void Awake()
    {
        UpdateParentScale();
    }

    private void Update()
    {
        GetComponent<Rigidbody>().mass = mass;
        transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        transform.localEulerAngles = new Vector3(axialTilt, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void RotateOnAxis(float timescale)
    {
        transform.localEulerAngles -= new Vector3(0, rotationSpeed * 360f * Time.deltaTime * timescale, 0);
    }

    public OrbitType GetOrbitType()
    {
        return setToOrbit;
    }

    public bool DidParentScaleChange()
    {
        return !(previousParentScale.Equals(transform.parent.localScale.x));
    }

    public float GetPreviousParentScale()
    {
        return previousParentScale;
    }

    public float GetDefaultScale()
    {
        return 1f;
    }

    public void UpdateParentScale()
    {
        previousParentScale = transform.parent.localScale.x;
    }
}
