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
    [SerializeField] int rowLength;
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

        for (int i = 0; i < gameTiles.Length; i++)
        {
            SetNeighbourTiles(i);
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
    void SetNeighbourTiles(int index)
    {
        Tile leftTile = null;
        Tile rightTile = null;
        Tile upTile = null;
        Tile downTile = null;
        if (index + 1 == gameTiles.Length)
        {
            rightTile = gameTiles[index - 1];
            downTile = gameTiles[index - rowLength];
        }
        else if (index == 0)
        {
            leftTile = gameTiles[index + 1];
            upTile = gameTiles[index + rowLength];
        }
        //Edges
        else
        {
            int[] tileGameIndexes = gameTiles[index].GetIndexes();
            //Right upper corner
            if (tileGameIndexes[0] == gameTilesRowsCount - 1 && tileGameIndexes[1] == 0)
            {
                leftTile = gameTiles[index + 1];
                downTile = gameTiles[index - rowLength];
            }
            //Left Lower Corner
            else if (tileGameIndexes[0] == 0 && tileGameIndexes[1] == rowLength - 1)
            {
                rightTile = gameTiles[index - 1];
                upTile = gameTiles[index + rowLength];
            }
            //Right Edge
            else if (tileGameIndexes[1] == 0)
            {
                leftTile = gameTiles[index + 1];
                upTile = gameTiles[index + rowLength];
                downTile = gameTiles[index - rowLength];
            }
            //Left Edge
            else if (tileGameIndexes[1] == rowLength - 1)
            {
                rightTile = gameTiles[index - 1];
                upTile = gameTiles[index + rowLength];
                downTile = gameTiles[index - rowLength];
            }
            //lower Edge
            else if (tileGameIndexes[0] == 0)
            {
                upTile = gameTiles[index + rowLength];
                leftTile = gameTiles[index + 1];
                rightTile = gameTiles[index - 1];
            }
            //upper Edge
            else if (tileGameIndexes[0] == gameTilesRowsCount - 1)
            {
                downTile = gameTiles[index - rowLength];
                leftTile = gameTiles[index + 1];
                rightTile = gameTiles[index - 1];
            }
            //middle area
            else
            {
                upTile = gameTiles[index + rowLength];
                downTile = gameTiles[index - rowLength];
                leftTile = gameTiles[index + 1];
                rightTile = gameTiles[index - 1];
            }
        }
            gameTiles[index].SetNeighbourTiles(leftTile, rightTile, upTile, downTile);
    }
}
