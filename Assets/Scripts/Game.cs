using System;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game Instance;

    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] int gameTilesRowsCount;
    [SerializeField] GameObject[] gameObjects;
    [SerializeField] Color selectedTileColor;
    [SerializeField] Color deselectedTileColor;
    Tile[] gameTiles;
    Tile selectedTile;
    void Start()
    {
        gameTiles = GetComponentsInChildren<Tile>();
        SetTilesGameIndex();
    }

    void SetTilesGameIndex()
    {
        int currentIndex = 0;
        int currentRow = 0;
        int rowLength = gameTiles.Length / gameTilesRowsCount;
        while (currentIndex < gameTiles.Length)
        {
            int i = 0;
            for (i = currentIndex; i < rowLength * (currentRow + 1); i++)
            {
                int randomGameObjectIndex = UnityEngine.Random.Range(0, gameObjects.Length);
                gameTiles[i].SetIndex(currentRow, i - currentIndex, gameObjects[randomGameObjectIndex]);
            }
            currentIndex = i;
            currentRow += 1;
        }
    }

    public void SelectTile(Tile tile)
    {
        if (selectedTile == null)
        {
            tile.GetComponent<Image>().color = selectedTileColor;
            selectedTile = tile;
        }
        else
        {
            //TODO: Check if a match can be made and cancel the swap if not possible

            int[] idxTileSelected = selectedTile.GetIndexes();
            int[] idxTileCurrent = tile.GetIndexes();

            //Check if beside the current tile (up,down,left,right)
            if (idxTileCurrent[0] == idxTileSelected[0] || idxTileCurrent[1] == idxTileSelected[1])
            {
                //Swap Tiles
                GameObject firstImage = selectedTile.GetTileObject();
                GameObject secondImage = tile.GetTileObject();

                firstImage.transform.SetParent(tile.transform);
                firstImage.transform.localPosition = new Vector2(0, 0);

                secondImage.transform.SetParent(selectedTile.transform);
                secondImage.transform.localPosition = new Vector2(0, 0);

                selectedTile.InitializeTileImage();
                tile.InitializeTileImage();

                selectedTile.GetComponent<Image>().color = deselectedTileColor;
                selectedTile = null;
            }
            else
            {
                //Cancel Selection
                selectedTile.GetComponent<Image>().color = deselectedTileColor;
                selectedTile = null;
            }
        }
    }
}
