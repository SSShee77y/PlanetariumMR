using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField]
    private Transform simulationSystem;

    [Header("Extra Flags")]
    [SerializeField]
    private bool disableChildrenOnStart;
    [SerializeField]
    private bool useExclusionList;
    [SerializeField]
    private bool makeListInclusive;
    [SerializeField]
    private List<Transform> exclusionList;

    private CelestialInfo[] celesials;

    private void Start()
    {
        if (simulationSystem == null)
            simulationSystem = transform;
        celesials = simulationSystem.gameObject.GetComponentsInChildren<CelestialInfo>();
        if (disableChildrenOnStart)
            DisableChildren();
    }

    private void DisableChildren()
    {
        foreach (Transform child in simulationSystem.GetComponentsInChildren<Transform>())
        {
            if (child == simulationSystem)
                continue;
            if (useExclusionList)
            {
                if (exclusionList.Contains(child) && makeListInclusive == false)
                {
                    continue;
                }
                else if (exclusionList.Contains(child) && makeListInclusive == true)
                {
                    child.gameObject.SetActive(false);
                    continue;
                }
            }
            child.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsGameObjectAChild(other.gameObject))
            return;

        int childNameIndex = GetCelestialNameIndex(other.transform.name);
        if (childNameIndex >= 0)
        {
            celesials[childNameIndex].gameObject.SetActive(true);
            Destroy(other.gameObject);
        }
    }

    public bool IsGameObjectAChild(GameObject gameObject)
    {
        foreach (Transform child in simulationSystem.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<CelestialInfo>() == null)
                continue;
            if (child == simulationSystem)
                continue;
            if (child.gameObject == gameObject)
                return true;
        }
        return false;
    }

    public int GetCelestialNameIndex(string name)
    {
        for (int i = 0; i < celesials.Length; i++)
        {
            if (celesials[i].name.Equals(name))
                return i;
        }
        return -1;
    }    
}
