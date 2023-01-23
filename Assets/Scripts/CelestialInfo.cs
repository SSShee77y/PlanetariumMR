using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialInfo : MonoBehaviour
{
    public float mass = 1; // Unit: Earth
    public float radius = 1; // Unit: IDK
    public float rotationSpeed = 1; // Unit: day
    public float initialSpeed = 10; // (Use for Orbital Speed) Unit: 1,000,000 km / day 
    public float semiMajorAxis = 1; // Unit: 1,000,000 km
    public GameObject orbitalPrimary = null;
    public enum orbitType { none, perfect, elliptic };
    [SerializeField] orbitType setToOrbit = orbitType.none; // Set type of orbit
    public float axialTilt = 0f; // Relative to earth's elliptic plane
    public float inclination = 0.0f; // Relative to earth's elliptic plane

    private float parentObjectScale = 1f;
    private float defaultObjectScale = 1f;

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

    public orbitType GetOrbitType()
    {
        return setToOrbit;
    }

    public bool DoScalesMatch()
    {
        return parentObjectScale.Equals(transform.parent.localScale.x);
    }

    public void UpdateScales()
    {
        parentObjectScale = transform.parent.localScale.x;
    }

    public float GetPreviousScale()
    {
        return parentObjectScale;
    }

    public float GetDefaultScale()
    {
        return defaultObjectScale;
    }
}
