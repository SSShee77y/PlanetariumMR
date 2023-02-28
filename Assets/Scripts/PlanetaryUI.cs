using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlanetaryUI : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private GameObject _celestialUIPrefab;
    [SerializeField] [Tooltip("The highlight box will not show up if the diameter is bigger than (this factor * screen height)")]
    private float _maxUISizeToScreenHeight = 0.5f;
    [SerializeField] [Tooltip("How close the camera must be to the object for UI to disappear")]
    private float _closeUpDistance = 0.4f;
    [SerializeField]
    private float _textOffset = 260f;

    [SerializeField]
    public float displayRatioScaler = 1.0f;
    
    private List<GameObject> _celestialUIList = new List<GameObject>();

    void Update()
    {
        ManageHighlightsAmount();
        UpdateHighlightBox();
    }

    void UpdateHighlightBox()
    {
        // Disable targetbox if the object is behind you relatively (negative relative z)

        List<CelestialInfo> celestials = new List<CelestialInfo>();
        celestials.AddRange(FindObjectsOfType<CelestialInfo>());

        for (int i = 0; i < _celestialUIList.Count && i < celestials.Count; i++)
        {
            GameObject celestial = celestials[i].gameObject;
            GameObject highlightBox = _celestialUIList[i];

            Vector3 relativeDisplacement = GetRelativeDisplacement(celestial.transform, _mainCamera.transform);

            if (relativeDisplacement.z <= 0 || relativeDisplacement.magnitude < _closeUpDistance)
            {
                highlightBox.SetActive(false);
                continue;
            }
            else
            {
                highlightBox.SetActive(true);
            }

            TextMeshProUGUI textMeshProUGUI = highlightBox.GetComponentInChildren<TextMeshProUGUI>();
            GameObject image = highlightBox.transform.GetChild(0).gameObject;

            textMeshProUGUI.text = celestial.name;

            float scaleWidth = RelativeWidth(relativeDisplacement, celestial.transform.lossyScale.x);
            scaleWidth = Mathf.Clamp(scaleWidth, 100f, _maxUISizeToScreenHeight * 4f * GetComponent<RectTransform>().sizeDelta.y); 
            if (scaleWidth >= _maxUISizeToScreenHeight * 4f * GetComponent<RectTransform>().sizeDelta.y)
            {
                highlightBox.SetActive(false);
                continue;
            }

            image.GetComponent<RectTransform>().sizeDelta = new Vector2(scaleWidth, scaleWidth);
            textMeshProUGUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(_textOffset + (scaleWidth / 2.0f), 0);

            highlightBox.GetComponent<RectTransform>().anchoredPosition = RelativeDisplacementToAnchor(relativeDisplacement);
        }
    }

    Vector3 GetRelativeDisplacement(Transform toTransform, Transform fromTransform)
    {
        Vector3 displacement = (toTransform.position - fromTransform.position);

        Vector3 relativeDisplacement = new Vector3();
        relativeDisplacement.z = Vector3.Dot(displacement, fromTransform.forward);
        relativeDisplacement.y = Vector3.Dot(displacement, fromTransform.up);
        relativeDisplacement.x = Vector3.Dot(displacement, fromTransform.right);

        return relativeDisplacement;
    }

    Vector2 RelativeDisplacementToAnchor(Vector3 relativeDisplacement)
    {
        float horizontalLength = (relativeDisplacement.x / relativeDisplacement.z);
        float verticalLength = (relativeDisplacement.y / relativeDisplacement.z);
        float screenSpaceWidth = GetComponent<RectTransform>().sizeDelta.x / displayRatioScaler;
        float fromUIToCamera = Vector3.Distance(_mainCamera.transform.position, transform.position);

        Vector2 newAnchorPosition = new Vector2(fromUIToCamera * screenSpaceWidth * horizontalLength, fromUIToCamera * screenSpaceWidth * verticalLength);
        return newAnchorPosition;
    }

    float RelativeWidth(Vector3 relativeDisplacement, float planetScale)
    {
        float horizontalLength = (relativeDisplacement.x / relativeDisplacement.z);
        float screenSpaceWidth = GetComponent<RectTransform>().sizeDelta.x / displayRatioScaler;
        float scaleLength = ((relativeDisplacement.x + planetScale * 5f) / relativeDisplacement.z);

        return (scaleLength - horizontalLength) * screenSpaceWidth;
    }

    void ManageHighlightsAmount()
    {
        List<CelestialInfo> celestials = new List<CelestialInfo>();
        celestials.AddRange(FindObjectsOfType<CelestialInfo>());

        while (_celestialUIList.Count < celestials.Count)
        {
            var newTargetBox = Instantiate(_celestialUIPrefab, transform);
            newTargetBox.transform.SetParent(gameObject.transform);
            _celestialUIList.Add(newTargetBox);
        }
        while (_celestialUIList.Count > celestials.Count)
        {
            int lastIndex = _celestialUIList.Count - 1;
            Destroy(_celestialUIList[lastIndex].gameObject);
            _celestialUIList.Remove(_celestialUIList[lastIndex]);
        }
    }
}
