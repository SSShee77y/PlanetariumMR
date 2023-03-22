using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRandomizeSpawner : MonoBehaviour
{
    [Header("Required Spawning Options")]
    [SerializeField]
    private Transform objectToSpawn;
    [SerializeField]
    public int amountToSpawn = 10;
    [SerializeField]
    private Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField]
    private float minimumDistanceFromOthers = 1.0f;
    [SerializeField]
    
    [Header("Optional Visual Options")]
    private bool useRandomRadii = false;
    [SerializeField]
    private Vector2 randomRadiiBounds = new Vector2(0.1f, 0.6f);
    [SerializeField]
    private bool useRandomMaterials = false;
    [SerializeField]
    private List<Material> materialsList = new List<Material>();

    private List<Transform> spawnedObjectsList = new List<Transform>();
    public List<Transform> SpawnedObjectsList => spawnedObjectsList;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    [ContextMenu("ResetAndRespawn")]
    public void ResetAndRespawn()
    {
        foreach (Transform spawnedObject in spawnedObjectsList)
        {
            if (spawnedObject != null)
                Destroy(spawnedObject.gameObject);
        }
        spawnedObjectsList.Clear();
        
        Invoke("Start", Time.deltaTime * 2);
    }

    void Start()
    {
        boxSize = new Vector3(Mathf.Abs(boxSize.x), Mathf.Abs(boxSize.y), Mathf.Abs(boxSize.z));
        SpawnRandomObjects();
        HidePlanets();
    }

    void SpawnRandomObjects()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            // Get random spawn position and check if valid according to collisions
            // (possible endless loop if bounds too small for amount generated)
            Vector3 spawnPosition = GetRandomizedSpawnPoint();
            bool unableToSpawn = false;
            for (int j = 0; Physics.CheckSphere(spawnPosition, minimumDistanceFromOthers); j++) // conidtion true == meaning there is collision
            {
                spawnPosition = GetRandomizedSpawnPoint();
                if (j > 10)
                {
                    Debug.LogWarning("Unable to spawn object | Too many objects or not enough space");
                    unableToSpawn = true;
                    break;
                }
            }
            if (unableToSpawn)
            {
                break;
            }
            
            // Spawn object
            var spawnedObject = Instantiate(objectToSpawn, spawnPosition, transform.rotation);
            spawnedObjectsList.Add(spawnedObject.transform);

            // Randomize name
            spawnedObject.name = RandomStarName();

            // Use random radii
            if (useRandomRadii)
            {
                float randomRadius = Random.Range(randomRadiiBounds.x, randomRadiiBounds.y);
                if (spawnedObject.GetComponent<CelestialInfo>() != null)
                {
                    spawnedObject.GetComponent<CelestialInfo>().radius = randomRadius;
                }
                else
                {
                    spawnedObject.transform.localScale = new Vector3(randomRadius * 2f, randomRadius * 2f, randomRadius * 2f);
                }
            }

            // Use random materials
            if (useRandomMaterials && materialsList.Count > 0)
            {
                int materialIndex = Random.Range(0, materialsList.Count);
                spawnedObject.GetComponent<MeshRenderer>().material = materialsList[materialIndex];
            }
        }
    }

    void HidePlanets()
    {
        if (GetComponent<HidePlanetsInRandomStars>() != null) 
            GetComponent<HidePlanetsInRandomStars>().HidePlanets();
    }

    Vector3 GetRandomizedSpawnPoint()
    {
        Vector3 randomBounds = new Vector3(Random.Range(-boxSize.x / 2f, boxSize.x / 2f),
                                           Random.Range(-boxSize.y / 2f, boxSize.y / 2f),
                                           Random.Range(-boxSize.z / 2f, boxSize.z / 2f));
                                           
        return transform.position + randomBounds;
    }

    string RandomStarName()
    {
        string name = "";
        name += (char)('A' + Random.Range (0,26));
        name += (char)('A' + Random.Range (0,26));
        name += '-';
        name += Random.Range(100, 99999);

        return name;
    } 
}
