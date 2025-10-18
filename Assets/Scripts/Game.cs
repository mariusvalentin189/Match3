using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    [SerializeField] int rowsCount;
    [SerializeField] int columnsCount;
    [SerializeField] GameObject[] gameObjects;
    [SerializeField] Color selectedTileColor;
    [SerializeField] Color deselectedTileColor;
    [SerializeField] Tile tileObject;
    Tile[,] board;
    Tile selectedTile;
    Tile secondTile;
    bool canMove = true;
    int[] matches;
    void Start()
    {
        matches = new int[columnsCount];
        board = new Tile[rowsCount, columnsCount];
        SpawnGameTiles();
    }
    void SpawnGameTiles()
    {
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                board[i,j] = Instantiate(tileObject, transform);    
            }
        }

        SpawnTilesImage();
    }

    void SpawnTilesImage()
    {
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                int randomGameObjectIndex = UnityEngine.Random.Range(0, gameObjects.Length);
                while (CheckMatchingTiles(randomGameObjectIndex, i, j))
                {
                    randomGameObjectIndex = UnityEngine.Random.Range(0, gameObjects.Length);
                }
                
                board[i,j].SetIndex(gameObjects[randomGameObjectIndex], i, j);
            }
        }

    }
    
    
    bool CheckMatchingTiles(int objectType, int rowIndex, int columnIndex)
    {
        if (rowIndex >= 2)
        {
            if (board[rowIndex - 1, columnIndex].TileImageId == objectType && board[rowIndex - 2, columnIndex].TileImageId == objectType)
            {
                return true;
            }
        }
        if (columnIndex >= 2)
        {
            if (board[rowIndex, columnIndex - 1].TileImageId == objectType && board[rowIndex, columnIndex - 2].TileImageId == objectType)
            {
                return true;
            }
        }
        return false;
    }
    bool CheckMatchingTilesAfterSwap(Tile tile)
    {
        int objectType = tile.TileImageId;
        int rowIndex = tile.XIndex;
        int columnIndex = tile.YIndex;
        bool found = false;
        //Column three match case but in between two tiles
        if (rowIndex >= 1 && rowIndex <= rowsCount - 2)
        {
            if (board[rowIndex - 1, columnIndex].TileImageId == objectType && board[rowIndex + 1, columnIndex].TileImageId == objectType)
            {
                tile.IsMatched = true;
                board[rowIndex - 1, columnIndex].IsMatched = true;
                board[rowIndex + 1, columnIndex].IsMatched = true;
                found = true;
            }
        }
        //Row three match case but in between two tiles (can't be both in between two tiles on row and column)
        if (!found && columnIndex >= 1 && columnIndex <= columnsCount - 2)
        {
            if (board[rowIndex, columnIndex - 1].TileImageId == objectType && board[rowIndex, columnIndex + 1].TileImageId == objectType)
            {
                tile.IsMatched = true;
                board[rowIndex, columnIndex - 1].IsMatched = true;
                board[rowIndex, columnIndex + 1].IsMatched = true;
                found = true;
            }
        }

        if (rowIndex >= 2)
        {
            if (board[rowIndex - 1, columnIndex].TileImageId == objectType && board[rowIndex - 2, columnIndex].TileImageId == objectType)
            {
                tile.IsMatched = true;
                board[rowIndex - 1, columnIndex].IsMatched = true;
                board[rowIndex - 2, columnIndex].IsMatched = true;
                found =  true;

                //Search opposite direction for one match (4 matching tiles case)
                if (rowIndex <= rowsCount - 2)
                {
                    if (board[rowIndex + 1, columnIndex].TileImageId == objectType)
                    {
                        board[rowIndex + 1, columnIndex].IsMatched = true;
                    }
                }
            }
        }
        if (rowIndex <= rowsCount - 3)
        {
            if (board[rowIndex + 1, columnIndex].TileImageId == objectType && board[rowIndex + 2, columnIndex].TileImageId == objectType)
            {
                tile.IsMatched = true;
                board[rowIndex + 1, columnIndex].IsMatched = true;
                board[rowIndex + 2, columnIndex].IsMatched = true;
                found = true;

                //Search opposite direction for one match (4 matching tiles case)
                if (rowIndex >= 1)
                {
                    if (board[rowIndex - 1, columnIndex].TileImageId == objectType)
                    {
                        board[rowIndex - 1, columnIndex].IsMatched = true;
                    }
                }
            }
        }
        if (columnIndex >= 2)
        {
            if (board[rowIndex, columnIndex - 1].TileImageId == objectType && board[rowIndex, columnIndex - 2].TileImageId == objectType)
            {
                tile.IsMatched = true;
                board[rowIndex, columnIndex - 1].IsMatched = true;
                board[rowIndex, columnIndex - 2].IsMatched = true;
                found = true;

                //Search opposite direction for one match (4 matching tiles case)
                if (columnIndex <= columnsCount - 2)
                {
                    if (board[rowIndex, columnIndex + 1].TileImageId == objectType)
                    {
                        board[rowIndex, columnIndex + 1].IsMatched = true;
                    }
                }
            }
        }
        if (columnIndex <= columnsCount - 3)
        {
            if (board[rowIndex, columnIndex + 1].TileImageId == objectType && board[rowIndex, columnIndex + 2].TileImageId == objectType)
            {
                secondTile.IsMatched = true;
                board[rowIndex, columnIndex + 1].IsMatched = true;
                board[rowIndex, columnIndex + 2].IsMatched = true;
                found = true;

                //Search opposite direction for one match (4 matching tiles case)
                if (columnIndex >= 1)
                {
                    if (board[rowIndex, columnIndex - 1].TileImageId == objectType)
                    {
                        board[rowIndex, columnIndex - 1].IsMatched = true;
                    }
                }
            }
        }
        return found;
    }
    public void SelectTile(Tile tile)
    {
        if (!canMove) return;

        //Temp (No tiles falling after matching for now)
        if (tile.TileImageObject == null)
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

            //Check if beside the current tile (up,down,left,right)
            if (selectedTile.XIndex == secondTile.XIndex || selectedTile.YIndex == secondTile.YIndex)
            {

                //Check if the first selected tile is the one to make the match
                if (CheckMatches())
                {
                    int[] directions = GetTilesDirection();

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

    bool CheckMatches()
    {
        bool found = false;

        //Temporary swap tiles colors
        TileImage tempImg = selectedTile.TileImageObject.GetComponent<TileImage>();
        selectedTile.SetTileImaget(secondTile.TileImageObject.GetComponent<TileImage>());
        secondTile.SetTileImaget(tempImg);

        if (CheckMatchingTilesAfterSwap(selectedTile))
        {
            found = true;
        }

        if (CheckMatchingTilesAfterSwap(secondTile))
        {
            found = true;
        }

        //Swap tiles back
        tempImg = selectedTile.TileImageObject.GetComponent<TileImage>();
        selectedTile.SetTileImaget(secondTile.TileImageObject.GetComponent<TileImage>());
        secondTile.SetTileImaget(tempImg);
        return found;
    }

    int[] GetTilesDirection()
    {
        int[] directions = new int[2];
        //Is on the same row
        // 0 - left, 1- right, 2-up, 3- down
        if (selectedTile.XIndex == secondTile.XIndex)
        {
            if (selectedTile.YIndex > secondTile.YIndex)
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
        else if (selectedTile.YIndex == secondTile.YIndex)
        {
            if (selectedTile.XIndex > secondTile.XIndex)
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

    IEnumerator MovePieces()
    {
        canMove = false;
        yield return new WaitForSeconds(0.3f);

        //Swap Tiles
        GameObject firstImage = selectedTile.TileImageObject;
        GameObject secondImage = secondTile.TileImageObject;

        firstImage.transform.SetParent(secondTile.transform);
        firstImage.transform.localPosition = new Vector2(0, 0);

        secondImage.transform.SetParent(selectedTile.transform);
        secondImage.transform.localPosition = new Vector2(0, 0);

        selectedTile.InitializeTileImage();
        secondTile.InitializeTileImage();

        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            { 
                if (board[i, j].IsMatched)
                {
                    matches[j]++;
                    DestroyImmediate(board[i,j].TileImageObject);
                    board[i, j].InitializeTileImage();
                    board[i, j].IsMatched = false;
                }
            }
        }
        BringPiecesDown();
        selectedTile = null;
        secondTile = null;
        canMove = true;

    }
    void BringPiecesDown()
    {
        for (int column = 0; column < columnsCount; column++)
        {
            if (matches[column] == 0)
                continue;

            for (int row = 0; row < rowsCount - matches[column]; row++)
            {
                if (board[row, column].TileImageObject != null)
                    continue;

                //Move tiles from up to down
                GameObject tileToMove = board[row + matches[column], column].TileImageObject;

                tileToMove.transform.SetParent(board[row, column].transform);
                tileToMove.transform.localPosition = new Vector2(0, 0);

                board[row, column].InitializeTileImage();
                board[row + matches[column], column].InitializeTileImage();

                //TODO: Spawn tiles on the last row
            }
        }
        matches = new int[columnsCount];
        canMove = true;
    }
}
