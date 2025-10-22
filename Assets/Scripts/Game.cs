using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class Game : MonoBehaviour
{
    public static Game Instance;

    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] int rowsCount;
    [SerializeField] int columnsCount;
    [SerializeField] GameObject[] tileObjects;
    [SerializeField] Color selectedTileColor;
    [SerializeField] Color deselectedTileColor;
    [SerializeField] Tile tilePrefab;
    [SerializeField] private RectTransform animationLayer; // Empty GameObject under Canvas (with no layout groups)
    Tile[,] board;
    Tile selectedTile;
    Tile secondTile;
    bool canMove = true;
    int[] matches;
    Canvas canvas;
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
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
                board[i,j] = Instantiate(tilePrefab, transform);    
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
                int randomGameObjectIndex = UnityEngine.Random.Range(0, tileObjects.Length);
                while (CheckMatchingTiles(randomGameObjectIndex, i, j))
                {
                    randomGameObjectIndex = UnityEngine.Random.Range(0, tileObjects.Length);
                }
                
                board[i,j].SetTileImage(tileObjects[randomGameObjectIndex], i, j);
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

                    //Deselect the selected tile
                    selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;;

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

    IEnumerator MovePieces()
    {
        //Restrict movement while the pieces are being swapped
        canMove = false;

        //Duration of tthe swap
        float duration = 0.2f;

        //Get the tiles rectransform
        RectTransform tile1 = selectedTile.TileImageObject.GetComponent<RectTransform>();
        RectTransform tile2 = secondTile.TileImageObject.GetComponent<RectTransform>();

        // Get original parents (tiles)
        Transform parentTile1 = tile1.parent;
        Transform parentTile2 = tile2.parent;

        // Get local positions relative to the animation layer
        Vector2 localPosTile1 = WorldToLocalInRect(tile1.position, animationLayer, canvas);
        Vector2 localPosTile2 = WorldToLocalInRect(tile2.position, animationLayer, canvas);

        // Unparent and move to animation layer
        tile1.SetParent(animationLayer, worldPositionStays: false);
        tile2.SetParent(animationLayer, worldPositionStays: false);

        // Set the new anchored positions
        tile1.anchoredPosition = localPosTile1;
        tile2.anchoredPosition = localPosTile2;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float lerpTime = Mathf.SmoothStep(0f, 1f, t);

            tile1.anchoredPosition = Vector2.Lerp(localPosTile1, localPosTile2, lerpTime);
            tile2.anchoredPosition = Vector2.Lerp(localPosTile2, localPosTile1, lerpTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final positions
        tile1.anchoredPosition = localPosTile2;
        tile2.anchoredPosition = localPosTile1;

        // Reparent back to their new tiles (swap)
        tile1.SetParent(parentTile2);
        tile1.transform.localPosition = Vector3.zero;
        tile2.SetParent(parentTile1);
        tile2.transform.localPosition = Vector3.zero;

        // Set the tile child object
        selectedTile.InitializeTileImage();
        secondTile.InitializeTileImage();

        //Destroy matched tiles and count the number of matches for each column
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                if (board[i, j].IsMatched)
                {
                    matches[j]++;
                    DestroyImmediate(board[i, j].TileImageObject);
                    board[i, j].InitializeTileImage();
                    board[i, j].IsMatched = false;
                }
            }
        }

        //BringPiecesDown();

        selectedTile = null;
        secondTile = null;
        canMove = true;

    }

    // Cconvert world position to anchored local position in a RectTransform
    Vector2 WorldToLocalInRect(Vector3 worldPos, RectTransform targetRect, Canvas canvas)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, worldPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out localPoint);
        return localPoint;
    }

    void BringPiecesDown()
    {
        //TODO: Change to not make use of the animation
        //if (!bringingTilesDown)
        //    StartCoroutine(BringTilesDown());

        matches = new int[columnsCount];
        canMove = true;
    }
    IEnumerator BringTilesDown()
    {
        for (int column = 0; column < columnsCount; column++)
        {
            if (matches[column] == 0)
                continue;

            for (int row = 0; row < rowsCount - matches[column]; row++)
            {
                if (board[row, column].TileImageObject != null)
                    continue;
 
                yield return new WaitForSeconds(0.3f);
                //Move tiles from up to down
                GameObject tileToMove = board[row + matches[column], column].TileImageObject;

                tileToMove.transform.SetParent(board[row, column].transform);
                tileToMove.transform.localPosition = new Vector2(0, 0);

                board[row, column].InitializeTileImage();
                board[row + matches[column], column].InitializeTileImage();
                //TODO: Spawn tiles on the last row
            }

        }
    }
}
