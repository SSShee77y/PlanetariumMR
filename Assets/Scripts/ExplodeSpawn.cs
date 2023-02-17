using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeSpawn : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem supernova;
    [SerializeField]
    private GameObject planetToSpawn;

    [ContextMenu("ExplodeStar")]
    public void ExplodeStar()
    {
        if (supernova != null)
        {
            var spawnedSupernova = Instantiate(supernova, transform.position, transform.rotation);
            Destroy(spawnedSupernova, 5.0f);
        }

        if (planetToSpawn != null)
        {
            Instantiate(planetToSpawn, transform.position, transform.rotation);
        }
        
        Destroy(gameObject);
    }
}
