using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    TileObject tileObject;
    int gameRowIndex;
    int gameColumnIndex;
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
        r[0] = gameRowIndex;
        r[1] = gameColumnIndex;
        return r;
    }
    public void SetIndex(int rowValue, int columnValue, GameObject tileObj)
    {
        gameRowIndex = rowValue;
        gameColumnIndex = columnValue;
        GameObject t = Instantiate<GameObject>(tileObj);
        tileObject = t.GetComponent<TileObject>();
        tileObject.transform.SetParent(this.transform);
        tileObject.transform.localPosition = new Vector2(0, 0);
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
