using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    TileObject tileObject;
    int rowIndex;
    int columnIndex;
    Tile leftTile;
    Tile rightTile;
    Tile upTile;
    Tile downTile;
    void Start()
    {
        tileObject = GetComponentInChildren<TileObject>();
    }
    public GameObject GetTileObject()
    {
        return tileObject.gameObject;
    }
    public int GetTileObjectId()
    {
        return tileObject.id;
    }
    public void SetTileObjectId(int id)
    {
        tileObject.id = id;
    }
    public int[] GetIndexes()
    {
        int[] r = new int[2];
        r[0] = rowIndex;
        r[1] = columnIndex;
        return r;
    }
    public void SetIndex(int rowValue, int columnValue, GameObject tileObj)
    {
        rowIndex = rowValue;
        columnIndex = columnValue;
        GameObject t = Instantiate<GameObject>(tileObj);
        tileObject = t.GetComponent<TileObject>();
        tileObject.transform.SetParent(this.transform);
        tileObject.transform.localPosition = new Vector2(0, 0);
    }
    public void SetNeighbourTiles(Tile left, Tile right, Tile up, Tile down)
    {
        leftTile = left;
        rightTile = right;
        upTile = up;
        downTile = down;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Game.Instance.SelectTile(this);
    }
    public void InitializeTileImage()
    {
        tileObject = transform.GetChild(0).gameObject.GetComponent<TileObject>();
    }
}
