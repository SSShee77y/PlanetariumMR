using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectRandomizeSpawner))]
public class HidePlanetsInRandomStars : MonoBehaviour
{
    [SerializeField]
    private Transform parentOfObjectsToHide;

    public List<Transform> planetsToHide = new List<Transform>();
    private ObjectRandomizeSpawner randomizeSpawner;
    private List<Transform> starsList;

    public void HidePlanets()
    {
        GetPlanetsToHide();
        randomizeSpawner = GetComponent<ObjectRandomizeSpawner>();
        starsList = randomizeSpawner.SpawnedObjectsList;
        HidePlanetsRandomly();
    }

    void GetPlanetsToHide()
    {
        foreach (Transform child in parentOfObjectsToHide.GetComponentsInChildren<Transform>())
        {
            if (child.parent == parentOfObjectsToHide)
            {
                planetsToHide.Add(child);
            }
        }
    }

    void HidePlanetsRandomly()
    {
        if (planetsToHide.Count > starsList.Count)
        {
            Debug.LogWarning("Unable to hide planets to stars, as there are less stars than there are planets.");
            return;
        }

        for (int i = 0; i < planetsToHide.Count; i++)
        {
            Transform planet = planetsToHide[i];
            if (planet == null)
                continue;
            planet.gameObject.SetActive(true);
        }

        for (int i = 0; i < planetsToHide.Count; i++)
        {
            Transform planet = planetsToHide[i];
            if (planet == null)
                continue;
            while (true)
            {
                int starsListIndex = Random.Range(0, starsList.Count);
                Transform star = starsList[starsListIndex];
                if (star.GetComponentsInChildren<CelestialInfo>().Length < 2)
                {
                    planet.transform.parent = star;
                    planet.transform.localPosition = new Vector3(0, 0, 0);
                    star.GetComponent<StarExploder>().planetToReveal = planet.gameObject;
                    break;
                }
            }
        }

        for (int i = 0; i < planetsToHide.Count; i++)
        {
            Transform planet = planetsToHide[i];
            if (planet == null)
                continue;
            planet.transform.parent = parentOfObjectsToHide;
            planet.gameObject.SetActive(false);
        }
        
    }

}
