using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    TileImage tileImage;
    int xIndex;
    int yIndex;
    bool isMatched;
    public int XIndex { get { return xIndex; } }
    public int YIndex { get { return yIndex; } }
    public bool IsMatched { get { return isMatched; } set { isMatched = value; } }
    public GameObject TileImageObject { get { return tileImage ? tileImage.gameObject : null; } }
    public int TileImageId { get { return tileImage.id; } }

    void Start()
    {
        tileImage = GetComponentInChildren<TileImage>();
    }

    public void SetTileImage(GameObject tileObj, int x, int y)
    {
        GameObject t = Instantiate(tileObj);
        tileImage = t.GetComponent<TileImage>();
        tileImage.transform.SetParent(transform);
        tileImage.transform.localPosition = new Vector2(0, 0);
        tileImage.transform.localScale = Vector2.one;
        xIndex = x;
        yIndex = y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Game.Instance.SelectTile(this);
    }
    public void InitializeTileImage()
    {
        if (transform.childCount == 0)
        {
            tileImage = null;
        }
        else
        {
            tileImage = transform.GetChild(0).gameObject.GetComponent<TileImage>();
        }
    }
    public void SetTileImaget(TileImage tileImg)
    {
        tileImage = tileImg;
    }
}
