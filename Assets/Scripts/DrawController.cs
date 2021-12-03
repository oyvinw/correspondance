using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawController : MonoBehaviour
{
    public GameObject brush;
    public Camera cam;
    public float brushSize = 0.1f;
    public bool enableDrawing;
    public RenderTexture renderTexture;

    private List<GameObject> brushStrokes;
    private byte[] symbolTex;
    private Texture2D texture2d;

    private void Start()
    {
        brushStrokes = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && enableDrawing)
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var go = Instantiate(brush, hit.point + Vector3.up * 0.1f, transform.rotation, transform);
                go.transform.localScale = Vector3.one * brushSize;

                brushStrokes.Add(go);
            }
        }
    }

    public void Save()
    {
        Texture2D tex = new Texture2D(1080, 1080, TextureFormat.RGBA4444, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        texture2d = tex;
        symbolTex = tex.EncodeToPNG();
    }

    public byte[] GetSymbolTexture()
    {
        return symbolTex;
    }
    public Texture2D GetSymbolTexture2D()
    {
        return texture2d;
    }

    internal void EraseDrawing()
    {
        foreach (var stroke in brushStrokes)
        {
            Destroy(stroke);
        }

        brushStrokes = new List<GameObject>();
    }
}
