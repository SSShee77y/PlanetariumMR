using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

public class Gravitation : MonoBehaviour
{
    [Tooltip("in kg")]
    private static readonly float EMass = 5.972f * Mathf.Pow(10, 24);
    [Tooltip("Instead of (m^3 / kg s^2), uses (Gm^3 / EarthMass days^2)")]
    private static readonly float G = (6.6743f * Mathf.Pow(10, -11)) / Mathf.Pow(10, 27) * EMass * Mathf.Pow(86400, 2);

    [Header("Global Settings")]
    private static bool _pauseAllSimulations;

    [Header("Simulation Settings")]
    [SerializeField]
    private bool _useChildrenOnly;
    [SerializeField] [Range(0.4167f, 365f)] [Tooltip("1 real life second = 1 unit scale = 1 simulation day")]
    private float _timescale = 1f;
    private float _lastTimescale = 1f;

    [Header("Trail Settings")]
    [SerializeField] 
    private float _trailWidth = 1f;
    [SerializeField] [Tooltip("TrailTime basically signifies the length of a trail | 1 Unit = 1 day")]
    private float _trailTime = 30f;

    private List<GameObject> _celestials = new List<GameObject>();

    void Start()
    {
        _lastTimescale = _timescale;
        GetComponent<TrailRenderer>().widthMultiplier = _trailWidth;
        GetComponent<TrailRenderer>().time = _trailTime / _timescale;
        Initialization();
    }

    private void Update()
    {   
        if (_pauseAllSimulations)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1.0f;

        CheckTrailRenderer();
    }

    [ContextMenu("ToggleGlobalSimulationPause")]
    public void ToggleGlobalSimulationPause()
    {
        _pauseAllSimulations = !_pauseAllSimulations;
    }

    private void CheckTrailRenderer()
    {
        bool needsUpdate = false;

        if (_trailWidth != GetComponent<TrailRenderer>().widthMultiplier)
        {
            GetComponent<TrailRenderer>().widthMultiplier = _trailWidth;
            needsUpdate = true;
        }

        if (_trailTime / _timescale != GetComponent<TrailRenderer>().time)
        {
            GetComponent<TrailRenderer>().time = _trailTime / _timescale;
            needsUpdate = true;
        }

        if (needsUpdate)
        {
            AddTrails();
        }
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
        foreach (GameObject bodyOne in _celestials)
        {
            foreach (GameObject bodyTwo in _celestials)
            {
                if (!bodyOne.Equals(bodyTwo))
                {
                    float mass1 = bodyOne.GetComponent<Rigidbody>().mass;
                    float mass2 = bodyTwo.GetComponent<Rigidbody>().mass;
                    float radius = Vector3.Distance(bodyOne.transform.localPosition, bodyTwo.transform.localPosition);

                    // Mass2 mass multiplier canceled out by G multiplier
                    float force = (float) ((G * (mass1) * mass2) / Mathf.Pow(radius, 2));

                    float sizeScaleMultiplier = Mathf.Sqrt(bodyOne.transform.parent.localScale.x);

                    force *= Mathf.Pow(_timescale * sizeScaleMultiplier, 2);

                    bodyOne.GetComponent<Rigidbody>().AddForce((bodyTwo.transform.position - bodyOne.transform.position).normalized * force);
                }
            }
        }
    }

    /// <summary>
    /// Adds each wanted Celestial object to the list and initializes each Celestial object with base componenets from the object controller that has the Gravitation script.
    /// </summary>
    void Initialization()
    {   
        if (_useChildrenOnly)
        {
            foreach (CelestialInfo celestialInfo in GetComponentsInChildren<CelestialInfo>())
            {
                _celestials.Add(celestialInfo.gameObject);
            }
        }   
        else
        {
            _celestials.AddRange(GameObject.FindGameObjectsWithTag("Celestial"));
        }
        AddTrails();
        InitialVelocity();
    }

    void AddTrails()
    {
        foreach (GameObject body in _celestials)
        {
            if (body.GetComponent<TrailRenderer>() == null)
                body.AddComponent<TrailRenderer>();
            ComponentUtility.CopyComponent(GetComponent<TrailRenderer>());
            ComponentUtility.PasteComponentValues(body.GetComponent<TrailRenderer>());
        }
    }

    void InitialVelocity()
    {
        foreach (GameObject bodyOne in _celestials)
        {
            if (bodyOne.GetComponent<CelestialInfo>().orbitalPrimary != null)
            {
                GameObject bodyTwo = bodyOne.GetComponent<CelestialInfo>().orbitalPrimary;
                SpeedCalculation(bodyOne, bodyTwo);
            }

            else foreach (GameObject bodyTwo in _celestials)
            {
                if (!bodyOne.Equals(bodyTwo))
                {
                    SpeedCalculation(bodyOne, bodyTwo);
                }
            }
        }
    }

    /// <summary>
    /// Method called to help the InitialVelocity method to calculate the speed of the object for orbit.
    /// </summary>
    void SpeedCalculation(GameObject bodyToCalculate, GameObject bodyAffecting)
    {
        float mass2 = bodyAffecting.GetComponent<Rigidbody>().mass;
        float radius = Vector3.Distance(bodyToCalculate.transform.localPosition, bodyAffecting.transform.localPosition);
        float speed = bodyToCalculate.GetComponent<CelestialInfo>().initialSpeed;

        if (bodyToCalculate.GetComponent<CelestialInfo>().GetOrbitType() == CelestialInfo.OrbitType.Elliptic)
            speed = Mathf.Sqrt(G * (mass2) * (2 / radius - 1 / bodyToCalculate.GetComponent<CelestialInfo>().semiMajorAxis));
        else if (bodyToCalculate.GetComponent<CelestialInfo>().GetOrbitType() == CelestialInfo.OrbitType.Perfect)
            speed = Mathf.Sqrt(G * (mass2) / radius);

        float sizeScaleMultiplier = bodyAffecting.transform.parent.localScale.x;
        speed *= _timescale * sizeScaleMultiplier;

        // Will always go Counter-Clockwise (Else just do -= in velocity)
        bodyToCalculate.transform.LookAt(bodyAffecting.transform);
        bodyToCalculate.GetComponent<Rigidbody>().velocity += bodyToCalculate.transform.right * speed; 
    }

    void UpdateCelestialsList() 
    {
        int size = _celestials.Count;
        List<GameObject> listToAdd = new List<GameObject>();
        foreach (GameObject body in _celestials)
        {
            listToAdd.Remove(body);
        }
        _celestials.AddRange(listToAdd);
        if (_celestials.Count > size)
        {
            for (int i = size; i < _celestials.Count; i++)
            {
                GameObject bodyOne = _celestials[i];
                bodyOne.AddComponent<TrailRenderer>();
                ComponentUtility.CopyComponent(GetComponent<TrailRenderer>());
                ComponentUtility.PasteComponentValues(bodyOne.GetComponent<TrailRenderer>());
                if (bodyOne.GetComponent<CelestialInfo>().orbitalPrimary != null)
                {
                    GameObject bodyTwo = bodyOne.GetComponent<CelestialInfo>().orbitalPrimary;
                    SpeedCalculation(bodyOne, bodyTwo);
                }
                else foreach (GameObject bodyTwo in _celestials)
                {
                    if (!bodyOne.Equals(bodyTwo))
                    {
                        SpeedCalculation(bodyOne, bodyTwo);
                    }
                }
            }
        }
    }

    GameObject[] FindCurrentCelestials()
    {
        GameObject[] celestialList = new  GameObject[0];
        if (_useChildrenOnly)
        {
            CelestialInfo[] celestialInfos = GetComponentsInChildren<CelestialInfo>();
            celestialList = new GameObject[celestialInfos.Length];
            for (int i = 0; i < celestialInfos.Length; i++)
            {
                celestialList[i] = celestialInfos[i].gameObject;
            }
        }   
        else
        {   
            celestialList = GameObject.FindGameObjectsWithTag("Celestial");
        }
        return celestialList;
    }

    void UpdateTimescale()
    {
        // Updates the speed of each celestial object when timescale changes
        if (_timescale != _lastTimescale)
        {
            foreach (GameObject body in _celestials)
            {
                Rigidbody rb = body.GetComponent<Rigidbody>();
                float multiplier = _timescale / _lastTimescale;
                rb.velocity *= multiplier;
            }
        }

        _lastTimescale = _timescale;
    }

    void ScaleCheck()
    {
        foreach (GameObject body in _celestials)
        {
            if (!body.GetComponent<CelestialInfo>().DoScalesMatch())
            {
                RecalculateSpeed(body);
            }
        }
    }

    void RecalculateSpeed(GameObject body)
    {
        Rigidbody rb = body.GetComponent<Rigidbody>();
        rb.velocity *= Mathf.Sqrt(body.transform.parent.localScale.x / body.GetComponent<CelestialInfo>().GetPreviousScale());
        body.GetComponent<CelestialInfo>().UpdateScales();
    }

}
