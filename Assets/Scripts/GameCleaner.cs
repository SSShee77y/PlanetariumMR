using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCleaner : MonoBehaviour
{
    [ContextMenu("CleanUpRandomizer")]
    public void CleanUpRandomizer()
    {
        var systems = FindObjectsOfType<Gravitation>();
        FindObjectOfType<ObjectRandomizeSpawner>().ResetAndRespawn();
        FindObjectOfType<Collector>().DisableChildren();
        foreach (Gravitation system in systems)
        {
            system.ResetSystemTrails();
        }
    }

    [ContextMenu("ReloadScene")]
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
