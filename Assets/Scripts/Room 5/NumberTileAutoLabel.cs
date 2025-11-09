using UnityEngine;
using TMPro;

[RequireComponent(typeof(NumberTile))]
public class NumberTileAutoLabel : MonoBehaviour
{
    [Header("Layout")]
    [Tooltip("Hrúbka odsadenia od povrchu kocky (kvôli Z-fightingu).")]
    public float faceOffset = 0.005f;       
    [Tooltip("Šírka/výška textovej plochy na tvári kocky v lokálnych jednotkách kocky.")]
    public float faceSize = 0.22f;          
    [Tooltip("Veľkosť fontu (autoSizing on, ale toto je max).")]
    public float fontSize = 0.22f;
    public Color fontColor = Color.black;

    NumberTile tile;

    void Awake()
    {
        tile = GetComponent<NumberTile>();
        BuildFaces();      
        UpdateTexts();     
    }

    void OnValidate()
    {
        if (!tile) tile = GetComponent<NumberTile>();
        UpdateTexts();
        FitFaces();
    }

    void BuildFaces()
    {
        var old = transform.Find("Faces");
        if (old) DestroyImmediate(old.gameObject);

        var root = new GameObject("Faces").transform;
        root.SetParent(transform, false);

        CreateFace(root, "Front", new Vector3(0, 0, 0.5f + faceOffset), Quaternion.identity);
        CreateFace(root, "Back", new Vector3(0, 0, -0.5f - faceOffset), Quaternion.Euler(0, 180, 0));

        CreateFace(root, "Right", new Vector3(0.5f + faceOffset, 0, 0), Quaternion.Euler(0, -90, 0));
        CreateFace(root, "Left", new Vector3(-0.5f - faceOffset, 0, 0), Quaternion.Euler(0, 90, 0));

        CreateFace(root, "Top", new Vector3(0, 0.5f + faceOffset, 0), Quaternion.Euler(-90, 0, 0));
        CreateFace(root, "Bottom", new Vector3(0, -0.5f - faceOffset, 0), Quaternion.Euler(90, 0, 0));

        FitFaces();
    }

    void CreateFace(Transform root, string name, Vector3 localPos, Quaternion localRot)
    {
        var go = new GameObject(name);
        go.transform.SetParent(root, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = localRot;

        var tmp = go.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = fontColor;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMax = fontSize * 100f;   
        tmp.fontSizeMin = tmp.fontSizeMax * 0.3f;
        tmp.text = "0";                      
        tmp.overflowMode = TextOverflowModes.Overflow;

        var r = tmp.rectTransform;
        r.sizeDelta = Vector2.one * (faceSize * 100f); 
    }

    void FitFaces()
    {
        var faces = transform.Find("Faces");
        if (!faces) return;

        foreach (Transform f in faces)
        {
            var tmp = f.GetComponent<TextMeshPro>();
            if (!tmp) continue;
            tmp.color = fontColor;

            var r = tmp.rectTransform;
            r.sizeDelta = Vector2.one * (faceSize * 100f);
            tmp.fontSizeMax = fontSize * 100f;

            Vector3 n = f.forward; 
            f.localPosition = n * (0.5f + Mathf.Sign(Vector3.Dot(n, Vector3.forward)) * 0f) + 
                              Vector3.Scale(n, new Vector3(1, 1, 1)) * faceOffset;            
        }
    }

    void UpdateTexts()
    {
        var faces = transform.Find("Faces");
        if (!faces) return;
        string t = tile ? tile.value.ToString() : "0";
        foreach (Transform f in faces)
        {
            var tmp = f.GetComponent<TextMeshPro>();
            if (tmp) tmp.text = t;
        }
    }
}
