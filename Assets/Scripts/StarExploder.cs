using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarExploder : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem supernova;
    [SerializeField]
    public GameObject planetToSpawn;
    [SerializeField]
    public GameObject planetToReveal;

    [ContextMenu("ExplodeStar")]
    public void ExplodeStar()
    {
        if (supernova != null)
        {
            var spawnedSupernova = Instantiate(supernova, transform.position, transform.rotation);
            Destroy(spawnedSupernova.gameObject, 5.0f);
        }

        if (planetToSpawn != null)
        {
            Instantiate(planetToSpawn, transform.position, transform.rotation);
        }

        if (planetToReveal != null)
        {
            if (planetToReveal.transform.parent == transform)
                planetToReveal.transform.parent = null;
            planetToReveal.SetActive(true);
        }
        
        Destroy(gameObject);
    }
}
