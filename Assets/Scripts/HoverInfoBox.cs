using UnityEngine;
using TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(ReadCelestialFromFile))]
public class HoverInfoBox : MonoBehaviour
{
    private enum TextAlignment
    {
        Top,
        Middle,
        Bottom
    }

    private enum HoverSide
    {
        Top,
        Left,
        Bottom
    }

    [Header("Text Options")]
    [SerializeField]
    private TextMeshPro textMesh;
    [SerializeField] [TextArea(4,20)]
    public string TextInfo;
    [SerializeField]
    private TextAlignment textAlignment = TextAlignment.Middle; 

    [Header("Container Options")]
    [SerializeField]
    private Transform backplate;
    [SerializeField] [Tooltip("The height scale per line of text. AE: 0.02 height scale per line")]
    private float heightPerLine = 0.011f;
    [SerializeField] [Tooltip("The height scale per vertical margin length")]
    private float heightPerMargin = 0.008f;
    [SerializeField] [Tooltip("Total width of the container")]
    private float containerWidth = 0.25f;
    [SerializeField] [Tooltip("Outline thickness of the container")]
    private float outlineThickness = 0.01f;

    [Header("Hover Options")]
    [SerializeField]
    private Transform objectToHover;
    [SerializeField]
    private bool faceCamera;
    [SerializeField]
    private HoverSide hoverSide = HoverSide.Left;
    [SerializeField] [Tooltip("How far away from the object it hovers")]
    private float hoverGap = 0.1f;
    [SerializeField] [Tooltip("Adding the scale of the object to hover to the already set hover gap")]
    private bool addObjectScaleForGap;
    [SerializeField] [Tooltip("How far foward from the object it hovers")]
    private float hoverFront = 0.1f;

    private void ApplyText()
    {
        if (textMesh == null)
            return;
        
        textMesh.text = TextInfo; // Instead, replace textInfo by reading a text document filled with planet info.
        textMesh.GetComponent<RectTransform>().sizeDelta = new Vector2(containerWidth * 100, 1);
        textMesh.transform.localPosition = new Vector3(0, 0, 0.001f);

        if (textAlignment == TextAlignment.Top)
        {
            textMesh.verticalAlignment = TMPro.VerticalAlignmentOptions.Top;
        }
        else if (textAlignment == TextAlignment.Bottom)
        {
            textMesh.verticalAlignment = TMPro.VerticalAlignmentOptions.Bottom;
        }
        else
        {
            textMesh.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
        }
    }

    private int GetLineCount(string str)
    {
        int count = 0;
        int index = 0;

        while (true)
        {
            index = str.IndexOf('\n', index);
            count++;
            if (index < 0)
                break;
            index++;
        }

        return count;
    }

    private void ApplyContainer()
    {
        if (backplate == null || textMesh == null)
            return;
        
        int lineCount = GetLineCount(TextInfo);
        float containerHeight = heightPerLine * lineCount + heightPerMargin * (textMesh.margin.y + textMesh.margin.w);
        backplate.localScale = new Vector3(containerWidth, containerHeight, outlineThickness);
        
        if (textAlignment == TextAlignment.Top)
        {
            backplate.localPosition = new Vector3(0, -containerHeight / 2, 0);
        }
        else if (textAlignment == TextAlignment.Bottom)
        {
            backplate.localPosition = new Vector3(0, containerHeight / 2, 0);
        }
        else
        {
            backplate.localPosition = new Vector3(0, 0, 0);
        }
    }

    private void ApplyHover()
    {
        if (objectToHover == null)
            return;

        transform.position = objectToHover.transform.position;

        float hoverAmount = hoverGap;
        if (addObjectScaleForGap)
            hoverAmount += objectToHover.transform.lossyScale.y / 2;
        
        if (hoverSide == HoverSide.Top)
        {
            if (textMesh != null)
                textMesh.transform.localPosition += new Vector3(0, hoverAmount, hoverFront);

            if (backplate != null)
                backplate.transform.localPosition += new Vector3(0, hoverAmount, hoverFront);
        }
        else if (hoverSide == HoverSide.Left)
        {
            if (textMesh != null)
                textMesh.transform.localPosition += new Vector3(hoverAmount, 0, hoverFront);

            if (backplate != null)
                backplate.transform.localPosition += new Vector3(hoverAmount, 0, hoverFront);
        }
        else if (hoverSide == HoverSide.Bottom)
        {
            if (textMesh != null)
                textMesh.transform.localPosition += new Vector3(0, -hoverAmount, hoverFront);

            if (backplate != null)
                backplate.transform.localPosition += new Vector3(0, -hoverAmount, hoverFront);
        }

        if (Application.isPlaying && faceCamera)
        {
            transform.LookAt(FindObjectOfType<Camera>().transform);
            textMesh.transform.LookAt(FindObjectOfType<Camera>().transform);
            backplate.transform.LookAt(FindObjectOfType<Camera>().transform);
        }
    }

    private void Update()
    {
        if (Application.isPlaying && objectToHover == null)
        {
            backplate.gameObject.SetActive(false);
            textMesh.gameObject.SetActive(false);
        } else if (Application.isPlaying) {
            backplate.gameObject.SetActive(true);
            textMesh.gameObject.SetActive(true);
        }

        ApplyText();
        ApplyContainer();
        ApplyHover();
    }

    public void SetObjectToHover(Transform obj)
    {
        objectToHover = obj;
        backplate.gameObject.SetActive(true);
        textMesh.gameObject.SetActive(true);
        SetInfoForObject();
    }

    public void ClearObjectToHover()
    {
        objectToHover = null;
        backplate.gameObject.SetActive(false);
        textMesh.gameObject.SetActive(false);
    }

    [ContextMenu("UpdateInfo")]
    public void SetInfoForObject()
    {
        ReadCelestialFromFile reader = GetComponent<ReadCelestialFromFile>();
        if (reader != null)
        {
            TextInfo = reader.GetPlanetInfo(objectToHover.name);
        }
    }

}
