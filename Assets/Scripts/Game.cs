using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    List<Tile> matchedTiles = new List<Tile>();
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
            tile.GetComponent<UnityEngine.UI.Image>().color = selectedTileColor;
            selectedTile = tile;
        }
        else
        {
            if (CheckMatchingTilesRow(selectedTile, tile) >= 2)
            {
                //Check if a match can be made and cancel the swap if not possible
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

                    selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;

                    if(matchedTiles.Count() >= 2)
                    {
                        foreach(Tile gameTile in matchedTiles)
                        {
                            Destroy(gameTile.GetTileObject());
                            gameTile.InitializeTileImage();
                        }
                        matchedTiles = new List<Tile>();
                        Destroy(tile.GetTileObject());
                        tile.InitializeTileImage();
                    }
                    selectedTile = null;
                }
                else
                {
                    //Cancel Selection
                    selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;
                    selectedTile = null;
                }
            }
            else
            {
                //Cancel Selection
                selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;
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
    int CheckMatchingTilesRow(Tile swappingTile, Tile swappedTile)
    {
        int matches = 0;
        Tile[] swappedTileNeighbouringTiles = swappedTile.GetNeighbouringTiles();
        int swappingTileId = swappingTile.GetTileObjectId();
        if (swappingTile != swappedTileNeighbouringTiles[2] && swappingTile != swappedTileNeighbouringTiles[3])
        {
            //Check Row left
            if (swappedTileNeighbouringTiles[2])
            {
                if (swappedTileNeighbouringTiles[2].GetTileObjectId() == swappingTileId)
                {
                    matches++;
                    matchedTiles.Add(swappedTileNeighbouringTiles[2]);
                    Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[2].GetNeighbouringTiles();
                    if (swappedTileNeighbouringTile1[2])
                        if (swappedTileNeighbouringTile1[2].GetTileObjectId() == swappingTileId)
                        {
                            matches++;
                            matchedTiles.Add(swappedTileNeighbouringTile1[2]);
                        }
                }
            }
            //Check Row right
            if (swappedTileNeighbouringTiles[3])
            {
                if (swappedTileNeighbouringTiles[3].GetTileObjectId() == swappingTileId)
                {
                    matches++;
                    matchedTiles.Add(swappedTileNeighbouringTiles[3]);
                    Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[3].GetNeighbouringTiles();
                    if (swappedTileNeighbouringTile1[3])
                        if (swappedTileNeighbouringTile1[3].GetTileObjectId() == swappingTileId)
                        {
                            matches++;
                            matchedTiles.Add(swappedTileNeighbouringTile1[3]);
                        }
                }
            }
            //Check down column if moving from up
            if (swappingTile == swappedTileNeighbouringTiles[0])
            {
                if (swappedTileNeighbouringTiles[1])
                {
                    if (swappedTileNeighbouringTiles[1].GetTileObjectId() == swappingTileId)
                    {
                        matches++;
                        matchedTiles.Add(swappedTileNeighbouringTiles[1]);
                        Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[1].GetNeighbouringTiles();
                        if (swappedTileNeighbouringTile1[1])
                            if (swappedTileNeighbouringTile1[1].GetTileObjectId() == swappingTileId)
                            {
                                matches++;
                                matchedTiles.Add(swappedTileNeighbouringTile1[1]);

                            }
                    }
                }
            }
            //Check up column when moving from down
            else if (swappingTile == swappedTileNeighbouringTiles[1])
            {
                if (swappedTileNeighbouringTiles[0])
                {
                    if (swappedTileNeighbouringTiles[0].GetTileObjectId() == swappingTileId)
                    {
                        matches++;  
                        matchedTiles.Add(swappedTileNeighbouringTiles[0]);
                        Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[0].GetNeighbouringTiles();
                        if (swappedTileNeighbouringTile1[0])
                            if (swappedTileNeighbouringTile1[0].GetTileObjectId() == swappingTileId)
                            {
                                matches++;
                                matchedTiles.Add(swappedTileNeighbouringTile1[0]);
                            }
                    }
                }
            }
        }
            return matches;
    }
    int CheckMatchingTilesColumn(Tile swappingTile, Tile swappedTile)
    {
        int matches = 0;
        Tile[] swappedTileNeighbouringTiles = swappedTile.GetNeighbouringTiles();
        int swappingTileId = swappingTile.GetTileObjectId();
        if (swappingTile != swappedTileNeighbouringTiles[0] && swappingTile != swappedTileNeighbouringTiles[1])
        {
            //Check column up
            if (swappedTileNeighbouringTiles[0])
            {
                if (swappedTileNeighbouringTiles[0].GetTileObjectId() == swappingTileId)
                {
                    matches++;
                    Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[0].GetNeighbouringTiles();
                    if (swappedTileNeighbouringTile1[0])
                        if (swappedTileNeighbouringTile1[0].GetTileObjectId() == swappingTileId)
                            matches++;
                }
            }
            //Check column down
            if (swappedTileNeighbouringTiles[1])
            {
                if (swappedTileNeighbouringTiles[1].GetTileObjectId() == swappingTileId)
                {
                    matches++;
                    Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[1].GetNeighbouringTiles();
                    if (swappedTileNeighbouringTile1[1])
                        if (swappedTileNeighbouringTile1[1].GetTileObjectId() == swappingTileId)
                            matches++;
                }
            }
            //Check right when moving from left
            if (swappingTile == swappedTileNeighbouringTiles[2])
            {
                if (swappedTileNeighbouringTiles[3])
                {
                    if (swappedTileNeighbouringTiles[3].GetTileObjectId() == swappingTileId)
                    {
                        matches++;
                        Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[3].GetNeighbouringTiles();
                        if (swappedTileNeighbouringTile1[3])
                            if (swappedTileNeighbouringTile1[3].GetTileObjectId() == swappingTileId)
                                matches++;
                    }
                }
            }
            //Check left when moving from right
            else if (swappingTile == swappedTileNeighbouringTiles[3])
            {
                if (swappedTileNeighbouringTiles[2])
                {
                    if (swappedTileNeighbouringTiles[2].GetTileObjectId() == swappingTileId)
                    {
                        matches++;
                        Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[2].GetNeighbouringTiles();
                        if (swappedTileNeighbouringTile1[2])
                            if (swappedTileNeighbouringTile1[2].GetTileObjectId() == swappingTileId)
                                matches++;
                    }
                }
            }
        }
        return matches;
    }
}
