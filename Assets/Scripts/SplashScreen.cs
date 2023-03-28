using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [Header("Image Fading")]
    public Image FadeImage;
    public float TimeToStart = 1f;
    public float FadeInTime = 0.5f;
    public float SplashScreenTime = 1f;
    public float FadeOutTime = 0.5f;
    [Header("Scene Loading")]
    public string SceneNameToLoadInto;

    private float _timeToStart;
    private float _fadeInTime;
    private float _splashScreenTime;
    private float _fadeOutTime;
    private bool _attemptedSceneLoad;

    void Start()
    {
        Color fixedColor = FadeImage.color;
        fixedColor.a = 1;
        FadeImage.color = fixedColor;
        FadeImage.CrossFadeAlpha(1f, 0f, true);

        _timeToStart = TimeToStart;
        _fadeInTime = FadeInTime;
        _splashScreenTime = SplashScreenTime;
        _fadeOutTime = FadeOutTime;
    }

    void Update()
    {
        if (_timeToStart > 0f)
        {
            _timeToStart -= Time.deltaTime;
        }
        else if (_fadeInTime > 0f)
        {
            FadeImage.CrossFadeAlpha(0f, FadeInTime, false);
            _fadeInTime -= Time.deltaTime;
        }
        else if (_splashScreenTime > 0f)
        {
            _splashScreenTime -= Time.deltaTime;
        }
        else if (_fadeOutTime > 0f)
        {
            FadeImage.CrossFadeAlpha(1f, FadeOutTime, false);
            _fadeOutTime -= Time.deltaTime;
        }
        else if (_attemptedSceneLoad == false)
        {
            if (SceneNameToLoadInto != null && SceneNameToLoadInto.Length > 0)
            {
                Invoke("LoadNextScene", 1f); // One second delay needed for complete animation cycle
            }
            _attemptedSceneLoad = true;
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneNameToLoadInto, LoadSceneMode.Single);
    }
}
