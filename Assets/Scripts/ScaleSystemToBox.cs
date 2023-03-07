using UnityEngine;

public class ScaleSystemToBox : MonoBehaviour
{
    [SerializeField]
    private bool isEnabled;
    [SerializeField]
    private bool useAverageDistanceInstead;
    [SerializeField]
    private Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    [SerializeField] [Tooltip("Higher number means faster scaling")]
    private float scalingFactor = 0.5f;

    [ContextMenu("SetNewScale")]
    public void SetNewScale()
    {
        if (!isEnabled) return;
        transform.localScale = defaultScale / 2f / Mathf.Pow(GetFarthestDistance(), scalingFactor);
    }

    private float GetFarthestDistance()
    {
        float farthestDistance = 0f;
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<CelestialInfo>() == null)
                continue;
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
