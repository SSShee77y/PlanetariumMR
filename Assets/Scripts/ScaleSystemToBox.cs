using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSystemToBox : MonoBehaviour
{
    [SerializeField]
    private bool isEnabled;
    [SerializeField]
    private bool useAverageDistanceInstead;
    [SerializeField]
    private float maxDiameter = 1f;
    [SerializeField]
    private Vector3 defaultScale = new Vector3(1f, 1f, 1f);

    [ContextMenu("SetNewScale")]
    public void SetNewScale()
    {
        if (!isEnabled) return;
        transform.localScale = defaultScale * (maxDiameter / 2f / GetFarthestDistance());
    }

    private float GetFarthestDistance()
    {
        float farthestDistance = 0f;
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child == transform)
                continue;

            float transformDistance = Vector3.Distance(new Vector3(0, 0, 0), child.transform.localPosition);
            if (useAverageDistanceInstead)
                transformDistance = child.GetComponent<CelestialInfo>().semiMajorAxis;
            transformDistance += (child.transform.localScale.x / 2f);
            if (transformDistance > farthestDistance)
                farthestDistance = transformDistance;
        }
        return farthestDistance;
    }

}
