using System;
using System.Collections;
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
    Tile[] gameTiles;
    Tile selectedTile;
    Tile secondTile;
    bool canMove = true;
    List<Tile> firstMatchSwap = new List<Tile>();
    List<Tile> secondMatchSwap = new List<Tile>();
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
        if (!canMove) return;

        //Temp (No tiles falling after matching for now)
        if (tile.GetTileObject() == null)
        {
            //Unselect if previously selected
            if (selectedTile)
            {
                selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;
                selectedTile = null;
            }
            return;
        }

        if (selectedTile == null)
        {
            //Select first tile
            tile.GetComponent<UnityEngine.UI.Image>().color = selectedTileColor;
            selectedTile = tile;
        }
        else
        {
            //Select second tile
            secondTile = tile;

            //Check if a match can be made and cancel the swap if not possible
            int[] idxTileSelected = selectedTile.GetIndexes();
            int[] idxTileCurrent = secondTile.GetIndexes();

            //Check if beside the current tile (up,down,left,right)
            if (idxTileCurrent[0] == idxTileSelected[0] || idxTileCurrent[1] == idxTileSelected[1])
            {

                //Check if the first selected tile is the one to make the match
                firstMatchSwap = CheckMatchingTilesRow(selectedTile, secondTile);
                if (firstMatchSwap.Count() >= 2)
                {
                    firstMatchSwap.Add(secondTile);
                }
                else
                {
                    firstMatchSwap = CheckMatchingTilesColumn(selectedTile, secondTile);
                    if (firstMatchSwap.Count() >= 2)
                    {
                        firstMatchSwap.Add(secondTile);
                    }
                }

                //Check if the second selected tile is the one to make the match
                secondMatchSwap = CheckMatchingTilesRow(secondTile, selectedTile);
                if (secondMatchSwap.Count() >= 2)
                {
                    secondMatchSwap.Add(selectedTile);
                }
                else
                {
                    secondMatchSwap = CheckMatchingTilesColumn(secondTile, selectedTile);
                    if (secondMatchSwap.Count() >= 2)
                    {
                        secondMatchSwap.Add(selectedTile);
                    }
                }
                //Check the matches for either the fist or second selected tile (including the selected tiles) is at least 3
                if (firstMatchSwap.Count() >= 3 || secondMatchSwap.Count() >= 3)
                {

                    //Get the direction the second tile is in relation to the first tile
                    //Direction are represented by int numbers: 0 - left, 1- right, 2-up, 3- down
                    int[] directions = GetTilesDirection(idxTileCurrent, idxTileSelected);

                    //Deselect the selected tile
                    selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;

                    //Play swap animation based on the direction
                    selectedTile.PlaySwapAnimation(directions[1]);
                    secondTile.PlaySwapAnimation(directions[0]);

                    //Start the Coroutine to handle destroying the mathed pieces
                    StartCoroutine(MovePieces());
                }
                else
                {
                    //Cancel Selection
                    selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;
                    selectedTile = null;
                    secondTile = null;
                }
            }

            else
            {
                //Cancel Selection
                selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;
                selectedTile = null;
                secondTile = null;
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
    
    int[] GetTilesDirection(int[] firstTileIndexes, int[] secondTileIndexes)
    {
        int[] directions = new int[2];
        //Is on the same row
        // 0 - left, 1- right, 2-up, 3- down
        if (firstTileIndexes[0] == secondTileIndexes[0])
        {
            if (firstTileIndexes[1] < secondTileIndexes[1])
            {
                directions[0] = 0;
                directions[1] = 1;
            }
            else
            {
                directions[0] = 1;
                directions[1] = 0;
            }
        }
        //Same column
        else if (firstTileIndexes[1] == secondTileIndexes[1])
        {
            if (firstTileIndexes[0] < secondTileIndexes[0])
            {
                directions[0] = 2;
                directions[1] = 3;
            }
            else
            {
                directions[0] = 3;
                directions[1] = 2;
            }
        }
        return directions;
    }
    List<Tile> CheckMatchingTilesRow(Tile swappingTile, Tile swappedTile)
    {
        int matches = 0;
        List<Tile> matchedTiles = new List<Tile>();
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
            //If only one match is found do not include it
            if (matches == 1)
            {
                matches = 0;
                matchedTiles = new List<Tile>();
            }
            //Check down column if moving from up
            if (swappingTile == swappedTileNeighbouringTiles[0])
            {
                if (swappedTileNeighbouringTiles[1])
                {
                    if (swappedTileNeighbouringTiles[1].GetTileObjectId() == swappingTileId)
                    {
                        Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[1].GetNeighbouringTiles();
                        if (swappedTileNeighbouringTile1[1])
                            if (swappedTileNeighbouringTile1[1].GetTileObjectId() == swappingTileId)
                            {
                                //Only of two matches are found, include them
                                matches+=2;
                                matchedTiles.Add(swappedTileNeighbouringTiles[1]);
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
                        Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[0].GetNeighbouringTiles();
                        if (swappedTileNeighbouringTile1[0])
                            if (swappedTileNeighbouringTile1[0].GetTileObjectId() == swappingTileId)
                            {
                                //Only of two matches are found, include them
                                matches += 2;
                                matchedTiles.Add(swappedTileNeighbouringTiles[0]);
                                matchedTiles.Add(swappedTileNeighbouringTile1[0]);
                            }
                    }
                }
            }
        }
            return matchedTiles;
    }
    List<Tile> CheckMatchingTilesColumn(Tile swappingTile, Tile swappedTile)
    {
        int matches = 0;
        List<Tile> matchedTiles = new List<Tile>();
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
            //Check column down
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
                            matchedTiles.Add(swappedTileNeighbouringTile1[1]);
                            matches++;
                        }
                }
            }
            //If only one match is found do not include it
            if (matches == 1)
            {
                matches = 0;
                matchedTiles = new List<Tile>();
            }
            //Check right when moving from left
            if (swappingTile == swappedTileNeighbouringTiles[2])
            {
                if (swappedTileNeighbouringTiles[3])
                {
                    if (swappedTileNeighbouringTiles[3].GetTileObjectId() == swappingTileId)
                    {  
                        Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[3].GetNeighbouringTiles();
                        if (swappedTileNeighbouringTile1[3])
                            if (swappedTileNeighbouringTile1[3].GetTileObjectId() == swappingTileId)
                            {
                                //Only of two matches are found, include them
                                matches += 2;
                                matchedTiles.Add(swappedTileNeighbouringTiles[3]);
                                matchedTiles.Add(swappedTileNeighbouringTile1[3]);
                            }
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
                        Tile[] swappedTileNeighbouringTile1 = swappedTileNeighbouringTiles[2].GetNeighbouringTiles();
                        if (swappedTileNeighbouringTile1[2])
                            if (swappedTileNeighbouringTile1[2].GetTileObjectId() == swappingTileId)
                            {
                                //Only of two matches are found, include them
                                matches += 2;
                                matchedTiles.Add(swappedTileNeighbouringTiles[2]);
                                matchedTiles.Add(swappedTileNeighbouringTile1[2]);
                            }
                    }
                }
            }
        }
        return matchedTiles;
    }

    IEnumerator MovePieces()
    {
        canMove = false;
        yield return new WaitForSeconds(0.3f);
        //Swap Tiles
        GameObject firstImage = selectedTile.GetTileObject();
        GameObject secondImage = secondTile.GetTileObject();

        firstImage.transform.SetParent(secondTile.transform);
        firstImage.transform.localPosition = new Vector2(0, 0);

        secondImage.transform.SetParent(selectedTile.transform);
        secondImage.transform.localPosition = new Vector2(0, 0);

        selectedTile.InitializeTileImage();
        secondTile.InitializeTileImage();

        if (firstMatchSwap.Count() >= 3)
        {
            foreach (Tile gameTile in firstMatchSwap)
            {
                Destroy(gameTile.GetTileObject());
                gameTile.InitializeTileImage();
            }
            firstMatchSwap = new List<Tile>();
        }
        if (secondMatchSwap.Count() >= 3)
        {
            foreach (Tile gameTile in secondMatchSwap)
            {
                Destroy(gameTile.GetTileObject());
                gameTile.InitializeTileImage();
            }
            secondMatchSwap = new List<Tile>();
        }
        selectedTile = null;
        secondTile = null;
        canMove = true;

    }
}
