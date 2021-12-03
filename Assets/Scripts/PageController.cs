using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
    public bool deletePagesFlag;
    public GameObject[] pagePrefabs;
    public GameObject[] cursedPagePrefabs;
    public GameObject[] introPrefabls;
    public GameObject[] endingPrefabs;
    public SpriteRenderer endLogo;

    private int pageIndex;
    private int layerIndex;
    private bool cursed;
    private bool firstCursed;
    private Canvas canvas;
    private List<GameObject> pages;
    private LetterController letterController;
    private DrawController drawController;
    public Color spriteColor = new Color(47, 53, 67);
    public GameObject introPage;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        pages = new List<GameObject>();
        letterController = FindObjectOfType<LetterController>();
        drawController = FindObjectOfType<DrawController>();
        deletePagesFlag = false;
        pages.Add(introPage);

        cursed = false;

        pageIndex = 0;
        layerIndex = 1;
    }

    private void Update()
    {
        if (deletePagesFlag)
        {
            foreach (var page in pages)
            {
                Destroy(page);
            }

            pages = new List<GameObject>();
            letterController.DeletePages();
            deletePagesFlag = false;
        }
    }

    public void DrawEndLogo()
    {
        var tex = drawController.GetSymbolTexture2D();

        //multiple?
        endLogo.enabled = true;

        var newSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.height, tex.width), new Vector2(0.5f, 0.5f), 100.0f);
        endLogo.sprite = newSprite;
        endLogo.transform.localScale = new Vector3(2.5f, 2.5f, 0f);
        endLogo.color = spriteColor;
    }

    public void Curse()
    {
        cursed = true;
        firstCursed = true;
    }

    public int TurnPage()
    {
        GameObject newPage;

        if (!cursed)
        {
            newPage = Instantiate(pagePrefabs[pageIndex % pagePrefabs.Length], canvas.transform);
        }
        else
        {
            if (firstCursed)
            {
                newPage = Instantiate(pagePrefabs[pageIndex % pagePrefabs.Length], canvas.transform);
                firstCursed = false;
            }
            else
            {
                newPage = Instantiate(cursedPagePrefabs[pageIndex % cursedPagePrefabs.Length], canvas.transform);
            }
        }

        var spriteRenderer = newPage.GetComponent<SpriteRenderer>();

        pages.Add(newPage);

        spriteRenderer.sortingOrder = layerIndex;
        layerIndex++;
        pageIndex++;

        var sprites = newPage.GetComponentsInChildren<SpriteRenderer>();
        var tex = drawController.GetSymbolTexture2D();

        foreach (var sprite in sprites)
        {
            if (sprite.tag == "symbol")
            {
                var newSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.height, tex.width), new Vector2(0.5f, 0.5f), 100.0f);
                sprite.sprite = newSprite;
                sprite.transform.localScale = new Vector3(Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.5f), 0f);
                sprite.sortingOrder = spriteRenderer.sortingOrder + 1;
                sprite.color = spriteColor;
            }
        }

        return layerIndex;
    }


}
