using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingScaler : MonoBehaviour
{
    [SerializeField]
    private float baseIntensity;
    [SerializeField]
    private float baseScale;
    [SerializeField]
    private Transform system;

    void Update()
    {
        if (GetComponent<Light>() != null)
        {
            GetComponent<Light>().intensity = Mathf.Clamp(Mathf.Pow(system.localScale.x / baseScale, 2) * baseIntensity, 0.001f, float.MaxValue);
        }
    }
}
