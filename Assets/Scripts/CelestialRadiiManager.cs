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

            if (radii.realisticRadius != 0f && radii.exaggeratedRadius != 0f && Application.isPlaying)
            {
                if (useExaggeratedRadius && radii.celestial.radius != radii.exaggeratedRadius)
                    radii.celestial.radius += ShiftScale(radii.realisticRadius, radii.exaggeratedRadius, 1f);
                else if (radii.celestial.radius != radii.realisticRadius)
                    radii.celestial.radius += ShiftScale(radii.exaggeratedRadius, radii.realisticRadius, 1f);
            }
            else
            {
                if (useExaggeratedRadius)
                    radii.celestial.radius = radii.exaggeratedRadius;
                else
                    radii.celestial.radius = radii.realisticRadius;
            }
        }
    }

    float ShiftScale(float fromScale, float toScale, float timeToShift)
    {
        return (toScale - fromScale) / timeToShift * Time.deltaTime;
    } 
}
