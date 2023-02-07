using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CelestialRadiiManager : MonoBehaviour
{
    [SerializeField]
    private bool useExaggeratedRadius;

    [Serializable]
    public struct CelestialRadii
    {
        public CelestialInfo celestial;
        public float realisticRadius;
        public float exaggeratedRadius;
    }

    [SerializeField]
    public List<CelestialRadii> CelestialRadiis = new List<CelestialRadii>();

    void Update()
    {
        for (int i = 0; i < CelestialRadiis.Count; i++)
        {
            CelestialRadii radii = CelestialRadiis[i];
            
            radii.realisticRadius = Mathf.Clamp(radii.realisticRadius, 0f, float.MaxValue);
            radii.exaggeratedRadius = Mathf.Clamp(radii.exaggeratedRadius, 0f, float.MaxValue);

            if (useExaggeratedRadius)
                radii.celestial.radius = radii.exaggeratedRadius;
            else
                radii.celestial.radius = radii.realisticRadius;
        }
    }

    float ShiftScale(float fromScale, float toScale, float timeToShift)
    {
        return (toScale - fromScale) / timeToShift * Time.deltaTime;
    } 
}
