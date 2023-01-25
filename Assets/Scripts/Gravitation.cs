using UnityEngine;
using UnityEditorInternal;

public class Gravitation : MonoBehaviour
{
    [Tooltip("in kg")]
    private static readonly float EMass = 5.972f * Mathf.Pow(10, 24);
    [Tooltip("Instead of (m^3 / kg s^2), uses (Gm^3 / EarthMass days^2)")]
    private static readonly float G = (6.6743f * Mathf.Pow(10, -11)) / Mathf.Pow(10, 27) * EMass * Mathf.Pow(86400, 2);

    [SerializeField] [Tooltip("1 real life second = 1 unit scale = 1 simulation day")]
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
        foreach (GameObject bodyOne in _celestials)
        {
            foreach (GameObject bodyTwo in _celestials)
            {
                if (!bodyOne.Equals(bodyTwo))
                {
                    float mass1 = bodyOne.GetComponent<Rigidbody>().mass;
                    float mass2 = bodyTwo.GetComponent<Rigidbody>().mass;
                    float radius = Vector3.Distance(bodyOne.transform.localPosition, bodyTwo.transform.localPosition);

                    float o1Multiplier = Mathf.Pow(bodyOne.transform.parent.localScale.x / bodyOne.GetComponent<CelestialInfo>().GetDefaultScale(), 3);

                    // Mass2 mass multiplier canceled out by G multiplier
                    float force = (float) ((G * (mass1) * mass2) / Mathf.Pow(radius, 2));

                    force *= _timescale * _timescale;

                    bodyOne.GetComponent<Rigidbody>().AddForce((bodyTwo.transform.position - bodyOne.transform.position).normalized * force);
                }
            }
        }
    }

    /// <summary>
    /// Initializes each Celestial object with base componenets from the object controller that has the Gravitation script.
    /// </summary>
    void Initialization()
    {
        foreach (GameObject body in _celestials)
        {
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

        float sizeScaleMultiplier = Mathf.Sqrt(bodyAffecting.transform.parent.localScale.x / bodyAffecting.GetComponent<CelestialInfo>().GetDefaultScale());
        speed *= _timescale * sizeScaleMultiplier;

        Debug.Log(string.Format("{0}, {1}, {2}", bodyToCalculate.name, bodyAffecting.name, radius));

        // Will always go Counter-Clockwise (Else just do -= in velocity)
        bodyToCalculate.transform.LookAt(bodyAffecting.transform);
        bodyToCalculate.GetComponent<Rigidbody>().velocity += bodyToCalculate.transform.right * speed; 
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
