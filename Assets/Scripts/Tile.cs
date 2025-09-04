using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TileObject tileObject;
    int gameRowIndex;
    int gameColumnIndex;
    GameObject tileImage;
    void Start()
    {
        tileObject = GetComponentInChildren<TileObject>();
        InitializeTileImage();
    }
    public GameObject GetTileImage()
    {
        return tileImage;
    }
    public int GetTileObjectId()
    {
        return tileObject.id;
    }
    public void SetTileObjectId(int id)
    {
        tileObject.id = id;
    }
    public void SetIndex(int rowValue, int columnValue)
    {
        gameRowIndex = rowValue;
        gameColumnIndex = columnValue;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Game.Instance.SelectTile(this);
    }
    public void InitializeTileImage()
    {
        tileImage = transform.GetChild(0).gameObject;
    }
}
