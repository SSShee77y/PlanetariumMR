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
    [SerializeField]
    private float _UISize = 1.0f;
    [SerializeField] [Tooltip("Number that represents how big the UI can take up the screen before disappearing")]
    private float _maxUIScreenScale = 5f;
    [SerializeField] [Tooltip("How close the camera must be to the object for UI to disappear")]
    private float _closeUpDistance = 0.4f;
    [SerializeField]
    private float _textOffset = 260f;
    
    private List<GameObject> _celestialUIList = new List<GameObject>();

    void Update()
    {
        ManageHighlightsAmount();
        UpdateHighlightBox();
    }

    void UpdateHighlightBox()
    {
        List<CelestialInfo> celestials = new List<CelestialInfo>();
        celestials.AddRange(FindObjectsOfType<CelestialInfo>());

        for (int i = 0; i < _celestialUIList.Count && i < celestials.Count; i++)
        {
            GameObject celestial = celestials[i].gameObject;
            GameObject highlightBox = _celestialUIList[i];

            Vector3 relativeDisplacement = GetRelativeDisplacement(celestial.transform, _mainCamera.transform);

            if (relativeDisplacement.z < _closeUpDistance)
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

            float scaleWidth = celestial.transform.lossyScale.x;
            if ((_UISize * scaleWidth) / (Mathf.Tan(_mainCamera.fieldOfView * Mathf.Deg2Rad) * relativeDisplacement.z) >= _maxUIScreenScale)
            {
                highlightBox.SetActive(false);
                continue;
            }

            highlightBox.GetComponent<RectTransform>().position = celestial.transform.position;
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(_UISize * scaleWidth * 100f, _UISize * scaleWidth * 100f);
            textMeshProUGUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(_textOffset + (_UISize * scaleWidth * 100f / 2.0f), 0);
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
