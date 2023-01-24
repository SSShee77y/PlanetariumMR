using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

public class Gravitation : MonoBehaviour
{
    private static readonly float EMass = (float)5.972 * Mathf.Pow(10, 24); // Unit: KG
    private static readonly float G = (float) (6.6743 * Mathf.Pow(10, -11)) / Mathf.Pow(10, 27) * EMass * Mathf.Pow(86400, 2); // (m^3 / kg s^2) to (Gm^3 / EM days^2)

    [SerializeField] 
    private float _timescale = 1f;
    private float _lastTimescale = 1f;
    [SerializeField] 
    private float _trailWidth = 1f;

    private GameObject[] _celestials;

    void Start()
    {
        _lastTimescale = _timescale;
        GetComponent<TrailRenderer>().widthMultiplier = _trailWidth;
        _celestials = GameObject.FindGameObjectsWithTag("Celestial");
        Initialization();
        InitialVelocity();
    }

    private void FixedUpdate() // Called 50 times per second
    {
        UpdateCelestialsList();
        UpdateTimescale();
        ScaleCheck();
        Gravity();
    }

    void Gravity()
    {
        foreach (GameObject o1 in _celestials)
        {
            foreach (GameObject o2 in _celestials)
            {
                if (!o1.Equals(o2))
                {
                    float mass1 = o1.GetComponent<Rigidbody>().mass;
                    float mass2 = o2.GetComponent<Rigidbody>().mass;
                    float radius = Vector3.Distance(o1.transform.position, o2.transform.position);

                    float o1Multiplier = Mathf.Pow(o1.transform.parent.localScale.x / o1.GetComponent<CelestialInfo>().GetDefaultScale(), 3);

                    // Mass2 mass multiplier canceled out by G multiplier
                    float force = (float) ((G * (o1Multiplier * mass1) * mass2) / Mathf.Pow(radius, 2));

                    force *= _timescale * _timescale;

                    o1.GetComponent<Rigidbody>().AddForce((o2.transform.position - o1.transform.position).normalized * force);
                }
            }
        }
    }

    /// <summary>
    /// Initializes each Celestial object with base componenets from the object controller that has the Gravitation script.
    /// </summary>
    void Initialization()
    {
        foreach (GameObject o in _celestials)
        {
            o.AddComponent<TrailRenderer>();
            ComponentUtility.CopyComponent(GetComponent<TrailRenderer>());
            ComponentUtility.PasteComponentValues(o.GetComponent<TrailRenderer>());
        }
    }

    void InitialVelocity()
    {
        foreach (GameObject o1 in _celestials)
        {
            if (o1.GetComponent<CelestialInfo>()._orbitalPrimary != null)
            {
                GameObject o2 = o1.GetComponent<CelestialInfo>()._orbitalPrimary;
                SpeedCalculation(o1, o2);
            }

            else foreach (GameObject o2 in _celestials)
            {
                if (!o1.Equals(o2))
                {
                    SpeedCalculation(o1, o2);
                }
            }
        }
    }

    /// <summary>
    /// Method called to help the InitialVelocity method to calculate the speed of the object for orbit.
    /// </summary>
    void SpeedCalculation(GameObject o1, GameObject o2)
    {
        float mass2 = o2.GetComponent<Rigidbody>().mass;
        float radius = Vector3.Distance(o1.transform.position, o2.transform.position);
        float speed = o1.GetComponent<CelestialInfo>()._initialSpeed;
        float o2Multiplier = Mathf.Pow(o2.transform.parent.localScale.x / o2.GetComponent<CelestialInfo>().GetDefaultScale(), 3);
        if (o1.GetComponent<CelestialInfo>().GetOrbitType() == CelestialInfo.OrbitType.elliptic)
            speed = Mathf.Sqrt(G * (o2Multiplier * mass2) * (2 / radius - 1 / o1.GetComponent<CelestialInfo>()._semiMajorAxis));
        else if (o1.GetComponent<CelestialInfo>().GetOrbitType() == CelestialInfo.OrbitType.perfect)
            speed = Mathf.Sqrt(G * (o2Multiplier * mass2) / radius);
        speed *= _timescale;

        Debug.Log(string.Format("{0}, {1}, {2}", o1.name, o2.name, radius));

        // Will always go Counter-Clockwise
        o1.transform.LookAt(o2.transform);
        o1.GetComponent<Rigidbody>().velocity += o1.transform.right * speed; 
    }

    void UpdateCelestialsList() 
    {
        // Checking speed calculations
        int size = _celestials.Length;
        _celestials = GameObject.FindGameObjectsWithTag("Celestial"); // Temporary solution before adding method that calls this everytime adding new body
        if (_celestials.Length > size)
        {
            for (int i = size; i < _celestials.Length; i++)
            {
                GameObject o1 = _celestials[i];
                o1.AddComponent<TrailRenderer>();
                ComponentUtility.CopyComponent(GetComponent<TrailRenderer>());
                ComponentUtility.PasteComponentValues(o1.GetComponent<TrailRenderer>());
                if (o1.GetComponent<CelestialInfo>()._orbitalPrimary != null)
                {
                    GameObject o2 = o1.GetComponent<CelestialInfo>()._orbitalPrimary;
                    SpeedCalculation(o1, o2);
                }
                else foreach (GameObject o2 in _celestials)
                {
                    if (!o1.Equals(o2))
                    {
                        SpeedCalculation(o1, o2);
                    }
                }
            }
        }
    }

    void UpdateTimescale()
    {
        // Updates the speed of each celestial object when timescale changes
        if (_timescale != _lastTimescale)
        {
            foreach (GameObject o in _celestials)
            {
                Rigidbody rb = o.GetComponent<Rigidbody>();
                float multiplier = _timescale / _lastTimescale;
                rb.velocity *= multiplier;
            }
        }

        _lastTimescale = _timescale;
    }

    void ScaleCheck()
    {
        foreach (GameObject o1 in _celestials)
        {
            if (!o1.GetComponent<CelestialInfo>().DoScalesMatch())
            {
                RecalculateSpeed(o1);
            }
        }
    }

    void RecalculateSpeed(GameObject o1)
    {
        Rigidbody rb = o1.GetComponent<Rigidbody>();
        rb.velocity *= Mathf.Pow(o1.transform.parent.localScale.x / o1.GetComponent<CelestialInfo>().GetPreviousScale(), 3/2);
        o1.GetComponent<CelestialInfo>().UpdateScales();
    }

}
