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

            Vector3 displacement = (celestial.transform.position - _mainCamera.transform.position);

            if (Vector3.Dot(displacement, _mainCamera.transform.forward) <= 0)
            {
                highlightBox.SetActive(false);
                continue;
            }
            else
            {
                highlightBox.SetActive(true);
            }

            highlightBox.GetComponentInChildren<TextMeshProUGUI>().text = celestial.name;

            highlightBox.GetComponent<RectTransform>().anchoredPosition = DisplacementToAnchorPosition(displacement);
        }
    }

    Vector3 DisplacementToAnchorPosition(Vector3 displacement)
    {
        Vector3 relativeDisplacement = new Vector3();
        relativeDisplacement.z = Vector3.Dot(displacement, _mainCamera.transform.forward);
        relativeDisplacement.y = Vector3.Dot(displacement, _mainCamera.transform.up);
        relativeDisplacement.x = Vector3.Dot(displacement, _mainCamera.transform.right);

        float horizontalLength = (relativeDisplacement.x / relativeDisplacement.z);
        float verticalLength = (relativeDisplacement.y / relativeDisplacement.z);
        float screenSpaceWidth = GetComponent<RectTransform>().sizeDelta.x / 2.05f;

        Vector2 newAnchorPosition = new Vector2(screenSpaceWidth * horizontalLength, screenSpaceWidth * verticalLength);
        return newAnchorPosition;
    }

    void ManageHighlightsAmount()
    {
        List<CelestialInfo> celestials = new List<CelestialInfo>();
        celestials.AddRange(FindObjectsOfType<CelestialInfo>());

        if (_celestialUIList.Count > celestials.Count)
        {
            int lastIndex = _celestialUIList.Count - 1;
            Destroy(_celestialUIList[lastIndex].gameObject);
            _celestialUIList.Remove(_celestialUIList[lastIndex]);
        }
        else if (_celestialUIList.Count < celestials.Count)
        {
            var newTargetBox = Instantiate(_celestialUIPrefab);
            newTargetBox.transform.SetParent(gameObject.transform);
            _celestialUIList.Add(newTargetBox);
        }
    }
}
