using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour
{
    [Header("Scene Loading")]
    public string SceneNameToLoadInto;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneNameToLoadInto, LoadSceneMode.Single);
    }
}
