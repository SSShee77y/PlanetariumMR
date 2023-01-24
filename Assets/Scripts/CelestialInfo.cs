using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialInfo : MonoBehaviour
{
    
    [Tooltip("1 Unit = 1 Earth Mass")]
    public float _mass = 1;
    [Tooltip("Units does not matter")]
    public float _radius = 1;
    [Tooltip("1 Unit = 1 day")]
    public float _rotationSpeed = 0;
    [Tooltip("1 Unit = 1,000,000 km / day")]
    public float _initialSpeed = 10; 
    [Tooltip("1 Unit = 1,000,000 km | Basically Average Distance from Sun")]
    public float _semiMajorAxis = 0;
    public GameObject _orbitalPrimary = null;
    
    public enum OrbitType
    {
        none,
        perfect,
        elliptic
    }

    [SerializeField]
    private OrbitType _setToOrbit = OrbitType.none;
    [Tooltip("Relative to earth's elliptic plane")]
    public float _axialTilt = 0f;
    [Tooltip("Relative to earth's elliptic plane")]
    public float _inclination = 0.0f;

    private float _parentObjectScale = 1f;
    private float _defaultObjectScale = 1f;

    void Awake()
    {
        UpdateScales();
        GetComponent<Rigidbody>().mass = _mass;
        transform.localScale = new Vector3(_radius * 2, _radius * 2, _radius * 2);
        transform.eulerAngles = new Vector3(_axialTilt, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void RotateOnAxis()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + _rotationSpeed / 50, transform.eulerAngles.z);
    }

    public OrbitType GetOrbitType()
    {
        return _setToOrbit;
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
