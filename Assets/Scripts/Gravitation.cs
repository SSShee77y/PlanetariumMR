using System.Collections.Generic;
using UnityEngine;

public class Gravitation : MonoBehaviour
{
    [Tooltip("in kg")]
    private static readonly float EMass = 5.972f * Mathf.Pow(10, 24);
    [Tooltip("Instead of (m^3 / kg s^2), uses (Gm^3 / EarthMass days^2)")]
    private static readonly float G = (6.6743f * Mathf.Pow(10, -11)) / Mathf.Pow(10, 27) * EMass * Mathf.Pow(86400, 2);

    [Header("Global Settings")]
    public static bool PauseAllSimulations;

    [Header("Simulation Settings")]
    [SerializeField]
    private bool _useChildrenOnly = true;
    [SerializeField] [Tooltip("Useful for stationary simulations with moving parts")]
    public bool useCenterOfMassCentering = true;
    [SerializeField] [Tooltip("1 real life second = 1 unit scale = 1 simulation day")]
    public float timescale = 1f;
    private float _lastTimescale = 1f;

    [Header("Trail Settings")]
    [SerializeField] 
    public float trailWidth = 1f;
    [SerializeField] [Tooltip("Trail time basically signifies the length of a trail | 1 Unit = length made by 1 simulation day")]
    public float trailTime = 30f;
    [SerializeField]
    private TrailRenderer _trailRendererPrefab;

    private List<GameObject> _celestials = new List<GameObject>();
    private float _lastCelestialsCount;
    private float _lastScale;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetCenterOfMass(), 0.04f);
    }

    void Start()
    {
        _lastTimescale = timescale;

        AddCelestialsToList(_celestials);
        AddTrailsToAll();
        CalculateSpeedOfAll();
    }

    private void Update()
    {    
        if (PauseAllSimulations)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1.0f;
    }

    [ContextMenu("ToggleGlobalSimulationPause")]
    public void ToggleGlobalSimulationPause()
    {
        PauseAllSimulations = !PauseAllSimulations;
    }

    private void FixedUpdate() // Called 50 times per second (Not affected by Time.timescale)
    {
        _celestials.Clear();
        AddCelestialsToList(_celestials);

        SetSystemScaleToBox();
        ManageTrails();
        UpdateTimescale();
        ScaleCheck();
        ApplyGravity();
        if (useCenterOfMassCentering)
            MoveObjectsToCenterOfSystem();
        RotateCelestialsOnAxis();

        _lastCelestialsCount = _celestials.Count;
        _lastScale = transform.localScale.x;
    }

    void MoveObjectsToCenterOfSystem()
    {
        Vector3 centerOfMass = GetCenterOfMass();
        Vector3 displacement = transform.position - centerOfMass;
        foreach (GameObject body in _celestials)
        {
            body.transform.position = body.transform.position + displacement;
        }
    }

    Vector3 GetCenterOfMass()
    {
        Vector3 totalDistance = new Vector3();
        float totalMass = 0f;
        foreach (GameObject body in _celestials)
        {
            float bodyMass = body.GetComponent<Rigidbody>().mass;
            totalMass += bodyMass;
            totalDistance += (bodyMass * body.transform.position);
        }

        return (totalDistance / totalMass);
    }

    void RotateCelestialsOnAxis()
    {
        foreach (GameObject body in _celestials)
        {
            body.GetComponent<CelestialInfo>().RotateOnAxis(timescale);
        }
    }

    void ApplyGravity()
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

                    force *= Mathf.Pow(timescale * sizeScaleMultiplier, 2);

                    bodyOne.GetComponent<Rigidbody>().AddForce((bodyTwo.transform.position - bodyOne.transform.position).normalized * force);
                }
            }
        }
    }

    void AddCelestialsToList(List<GameObject> list)
    {
        if (_useChildrenOnly)
        {
            foreach (CelestialInfo celestialInfo in GetComponentsInChildren<CelestialInfo>())
            {
                list.Add(celestialInfo.gameObject);
            }
        }   
        else
        {
            list.AddRange(GameObject.FindGameObjectsWithTag("Celestial"));
        }
    }

    void ManageTrails()
    {
        if (_lastCelestialsCount != _celestials.Count || transform.localScale.x != _lastScale)
        {
            ResetSystemTrails();
            Invoke("ResetSystemTrails", Time.deltaTime);
            Invoke("ResetSystemTrails", Time.deltaTime * 2f);
        }
        else
        {
            AddTrailsToAll();
        }
    }

    void AddTrailsToAll()
    {
        foreach (GameObject body in _celestials)
        {
            AddTrailToBody(body);
        }
    }

    void AddTrailToBody(GameObject body)
    {
        if (body.GetComponentInChildren<TrailRenderer>() == null)
            Instantiate(_trailRendererPrefab, body.transform);

        body.GetComponentInChildren<TrailRenderer>().widthMultiplier = trailWidth;
        body.GetComponentInChildren<TrailRenderer>().time = trailTime / timescale;

        body.GetComponentInChildren<TrailRenderer>().enabled = true;
    }

    public void ResetSystemTrails()
    {
        foreach (TrailRenderer trailRenderer in GetComponentsInChildren<TrailRenderer>())
        {
            trailRenderer.time = 0f;
        }
    }

    void CalculateSpeedOfAll()
    {
        foreach (GameObject bodyOne in _celestials)
        {
            CalculateSpeedOfBody(bodyOne);
        }
    }

    void CalculateSpeedOfBody(GameObject bodyOne)
    {
        if (bodyOne.GetComponent<CelestialInfo>().orbitalPrimary != null)
        {
            GameObject bodyTwo = bodyOne.GetComponent<CelestialInfo>().orbitalPrimary;
            SpeedCalculation(bodyOne, bodyTwo);
        }
        else foreach (GameObject bodyTwo in _celestials)
        {
            if (!bodyOne.Equals(bodyTwo))
                SpeedCalculation(bodyOne, bodyTwo);
        }
    }

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
        speed *= timescale * sizeScaleMultiplier;

        // Will always go Counter-Clockwise (Else just do -= in velocity)
        Quaternion previousRotation = bodyToCalculate.transform.rotation;
        bodyToCalculate.transform.LookAt(bodyAffecting.transform);
        bodyToCalculate.GetComponent<Rigidbody>().velocity += bodyToCalculate.transform.right * speed; 
        bodyToCalculate.transform.rotation = previousRotation;
    }

    void UpdateTimescale()
    {
        // Updates the speed of each celestial object when timescale changes
        if (timescale != _lastTimescale)
        {
            foreach (GameObject body in _celestials)
            {
                Rigidbody rb = body.GetComponent<Rigidbody>();
                float multiplier = timescale / _lastTimescale;
                rb.velocity *= multiplier;
            }
        }

        _lastTimescale = timescale;
    }

    void ScaleCheck()
    {   
        foreach (GameObject body in _celestials)
        {
            if (body.GetComponent<CelestialInfo>().DidParentScaleChange())
            {
                ScaleSpeedOfBody(body);
            }
        }
    }

    void ScaleSpeedOfBody(GameObject body)
    {
        Rigidbody rb = body.GetComponent<Rigidbody>();
        rb.velocity *= body.transform.parent.localScale.x / body.GetComponent<CelestialInfo>().GetPreviousParentScale();
        body.GetComponent<CelestialInfo>().UpdateParentScale();
    }

    void SetSystemScaleToBox()
    {
        if (GetComponent<ScaleSystemToBox>() != null)
            GetComponent<ScaleSystemToBox>().SetNewScale();
    }

}
