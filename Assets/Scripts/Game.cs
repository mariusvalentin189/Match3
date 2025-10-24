using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: Shuffle pieces when no matches can be done
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
    [SerializeField] Color selectedTileColor; //Tile color for when selected
    [SerializeField] Color deselectedTileColor; //Tile color for when deselected
    [SerializeField] Tile tilePrefab; // Tile to spawn prefab
    [SerializeField] RectTransform animationLayer; // Empty GameObject under Canvas (with no layout groups)
    [SerializeField] float duration = 0.2f; // Tiles swap or fall duration
    [SerializeField] float yDistanceBetweenTwoTiles = 60; //y distance in between two tiles (grid layout spacing + tiles size)
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
        SpawnGrid();
    }

    //Spawn the grid where the pieces will be placed  
    void SpawnGrid()
    {
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                board[i,j] = Instantiate(tilePrefab, transform);    
            }
        }
        SpawnPieces();
    }

    //Spawn the pieces
    void SpawnPieces()
    {
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                int randomGameObjectIndex = UnityEngine.Random.Range(0, tileObjects.Length);
                while (CheckMatchingTilesOnInitialize(randomGameObjectIndex, i, j))
                {
                    randomGameObjectIndex = UnityEngine.Random.Range(0, tileObjects.Length);
                }
                
                board[i,j].SetTileImage(tileObjects[randomGameObjectIndex], i, j);
            }
        }

    }
    
    //Check if the tile spawned creates a match and choose another random piece
    bool CheckMatchingTilesOnInitialize(int objectType, int rowIndex, int columnIndex)
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

    //Select the tiles to swap
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
                if (CheckSelectedTilesMatching())
                {

                    //Deselect the selected tile
                    selectedTile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor; ;

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

    //Check the newly spawned pieces after creating a match
    bool CheckMatchingTilesTilesSpawn(Tile tile)
    {
        int objectType = tile.TileImageId;
        int rowIndex = tile.XIndex;
        int columnIndex = tile.YIndex;
        bool found = false;
        if (rowIndex >= 2)
        {
            if (board[rowIndex - 1, columnIndex].TileImageId == objectType && board[rowIndex - 2, columnIndex].TileImageId == objectType)
            {
                tile.IsMatched = true;
                board[rowIndex - 1, columnIndex].IsMatched = true;
                board[rowIndex - 2, columnIndex].IsMatched = true;
                found = true;
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
            }
        }
        if (columnIndex <= columnsCount - 3)
        {
            if (board[rowIndex, columnIndex + 1].TileImageId == objectType && board[rowIndex, columnIndex + 2].TileImageId == objectType)
            {
                tile.IsMatched = true;
                board[rowIndex, columnIndex + 1].IsMatched = true;
                board[rowIndex, columnIndex + 2].IsMatched = true;
                found = true;
            }
        }

        return found;
    }

    //Check for matches before trying to make a swap 
    bool CheckMatchingTilesBeforeSwap(Tile tile)
    {
        int objectType = tile.TileImageId;
        int rowIndex = tile.XIndex;
        int columnIndex = tile.YIndex;
        bool found = false;
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
                tile.IsMatched = true;
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
        //Row three match case but in between two tiles
        if (columnIndex >= 1 && columnIndex <= columnsCount - 2)
        {
            if (board[rowIndex, columnIndex - 1].TileImageId == objectType && board[rowIndex, columnIndex + 1].TileImageId == objectType)
            {
                tile.IsMatched = true;
                board[rowIndex, columnIndex - 1].IsMatched = true;
                board[rowIndex, columnIndex + 1].IsMatched = true;
                found = true;
            }
        }
        return found;
    }
   
    //Check if the two pieces selected can match
    bool CheckSelectedTilesMatching()
    {
        bool found = false;

        //Temporary swap tiles colors
        TileImage tempImg = selectedTile.TileImageObject.GetComponent<TileImage>();
        selectedTile.SetTileImaget(secondTile.TileImageObject.GetComponent<TileImage>());
        secondTile.SetTileImaget(tempImg);

        if (CheckMatchingTilesBeforeSwap(selectedTile))
        {
            found = true;
        }

        if (CheckMatchingTilesBeforeSwap(secondTile))
        {
            found = true;
        }

        //Swap tiles back
        tempImg = selectedTile.TileImageObject.GetComponent<TileImage>();
        selectedTile.SetTileImaget(secondTile.TileImageObject.GetComponent<TileImage>());
        secondTile.SetTileImaget(tempImg);
        return found;
    }

    //Move the pieces with an animation
    IEnumerator MovePieces()
    {
        //Restrict movement while the pieces are being swapped
        canMove = false;

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
                if (!board[i, j].IsMatched)
                    continue;
                matches[j]++;
                DestroyImmediate(board[i, j].TileImageObject);
                board[i, j].InitializeTileImage();
            }
        }
        BringPiecesDown();
        matches = new int[columnsCount];
        ResetBoard();
        secondTile = null;
        selectedTile = null;
        while (!CheckIfTilesSpawned())
        {
            yield return null;
        }
        bool newMatchesFound;
        yield return new WaitForSeconds(0.5f);
        int steps = 0;
        do
        {
            steps++;
            newMatchesFound = false;
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    if (CheckMatchingTilesTilesSpawn(board[i, j]))
                    {
                        newMatchesFound = true;
                        board[i, j].IsMatched = true;
                    }
                }

            }
            if (newMatchesFound)
            {
                //Destroy matched tiles and count the number of matches for each column
                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < columnsCount; j++)
                    {
                        if (!board[i, j].IsMatched)
                            continue;
                        matches[j]++;
                        DestroyImmediate(board[i, j].TileImageObject);
                        board[i, j].InitializeTileImage();
                    }
                }
                BringPiecesDown();
                matches = new int[columnsCount];
                ResetBoard();

                while (!CheckIfTilesSpawned())
                {
                    yield return null;
                }
                yield return new WaitForSeconds(0.5f);
            }
            if(steps >= 200)
            {
                print("Infinite loop detected");
                break;
            }
        } while (newMatchesFound);
        canMove = true;
    }

    // Cconvert world position to anchored local position in a RectTransform
    Vector2 WorldToLocalInRect(Vector3 worldPos, RectTransform targetRect, Canvas canvas)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, worldPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out localPoint);
        return localPoint;
    }

    //Bring pieces down after making a match
    void BringPiecesDown()
    {
        for(int column = 0; column < columnsCount; column++)
        {
            if (matches[column] == 0)
                continue;

            int matchedFirstIndex = -1;
            int matchedPositionsOffest = -1;
            for (int row = 0; row < rowsCount; row++)
            {
                if (board[row, column].IsMatched)
                {
                    matchedFirstIndex = row + matches[column];
                    matchedPositionsOffest = matches[column];
                    StartCoroutine(AnimatePiecesDown(matchedFirstIndex, column, matchedPositionsOffest, duration / matches[column]));
                    break;
                }
            }
        }
        SpawnTilesOnTop();
    }
    
    //Animate the movement of pieces comming down
    IEnumerator AnimatePiecesDown(int matchedFirstIndex, int column, int matchedPositionsOffest, float duration)
    {
        float elapsed = 0f;
        List<Vector2> licalPosTiles1 = new List<Vector2>();
        List<Vector2> licalPosTiles2 = new List<Vector2>();
        List<RectTransform> tiles = new List<RectTransform>();
        List<Transform> parentTiles = new List<Transform>();
        for (int row = matchedFirstIndex; row < rowsCount; row++)
        {

            //Get the tiles rectransform
            RectTransform tile1 = board[row, column].TileImageObject.GetComponent<RectTransform>();
            RectTransform tile2 = board[row - matchedPositionsOffest, column].GetComponent<RectTransform>();


            // Get local positions relative to the animation layer
            Vector2 localPosTile1 = WorldToLocalInRect(tile1.position, animationLayer, canvas);
            Vector2 localPosTile2 = WorldToLocalInRect(tile2.position, animationLayer, canvas);

            // Unparent and move to animation layer
            tile1.SetParent(animationLayer, worldPositionStays: false);

            // Set the new anchored positions
            tile1.anchoredPosition = localPosTile1;

            licalPosTiles1.Add(localPosTile1);
            licalPosTiles2.Add(localPosTile2);
            tiles.Add(tile1);
            parentTiles.Add(tile2);

        }
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float lerpTime = Mathf.SmoothStep(0f, 1f, t);

            for (int tileIndex = 0; tileIndex < licalPosTiles1.Count(); tileIndex++)
            {

                tiles[tileIndex].anchoredPosition = Vector2.Lerp(licalPosTiles1[tileIndex], licalPosTiles2[tileIndex], lerpTime);

                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        for (int tileIndex = 0; tileIndex < licalPosTiles1.Count(); tileIndex++)
        {
            // Set to final positions
            tiles[tileIndex].anchoredPosition = licalPosTiles2[tileIndex];

            // Reparent back to the new tile
            tiles[tileIndex].SetParent(parentTiles[tileIndex]);
            tiles[tileIndex].transform.localPosition = Vector3.zero;

            // Set the tile child object
            parentTiles[tileIndex].GetComponent<Tile>().InitializeTileImage();
        }
    }

    //Check if all tiles spawned
    bool CheckIfTilesSpawned()
    {
        for(int i = 0; i < rowsCount; i++)
        {
            for(int j=0; j < columnsCount; j++)
            {
                if (board[i, j].TileImageObject == null)
                    return false;
            }
        }
        return true;
    }

    //Animate the spawned pieces to fall
    IEnumerator AnimateSpawnedPieces(List<GameObject> tilePieces, List<Tile> tileParents, float duratioo)
    {
        float elapsed = 0f;
        List<Vector2> licalPosTiles1 = new List<Vector2>();
        List<Vector2> licalPosTiles2 = new List<Vector2>();
        List<RectTransform> tiles = new List<RectTransform>();
        List<Transform> parentTiles = new List<Transform>();
        for (int i = 0; i < tilePieces.Count(); i++)
        {

            //Get the tiles rectransform
            RectTransform tile1 = tilePieces[i].GetComponent<RectTransform>();
            RectTransform tile2 = tileParents[i].GetComponent<RectTransform>();


            // Get local positions relative to the animation layer
            Vector2 localPosTile1 = WorldToLocalInRect(tile1.position, animationLayer, canvas);
            Vector2 localPosTile2 = WorldToLocalInRect(tile2.position, animationLayer, canvas);

            // Unparent and move to animation layer
            tile1.SetParent(animationLayer, worldPositionStays: false);

            // Set the new anchored positions
            tile1.anchoredPosition = localPosTile1;

            licalPosTiles1.Add(localPosTile1);
            licalPosTiles2.Add(localPosTile2);
            tiles.Add(tile1);
            parentTiles.Add(tile2);

        }

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float lerpTime = Mathf.SmoothStep(0f, 1f, t);

            for (int tileIndex = 0; tileIndex < licalPosTiles1.Count(); tileIndex++)
            {

                tiles[tileIndex].anchoredPosition = Vector2.Lerp(licalPosTiles1[tileIndex], licalPosTiles2[tileIndex], lerpTime);

                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        for (int tileIndex = 0; tileIndex < licalPosTiles1.Count(); tileIndex++)
        {
            // Set to final positions
            tiles[tileIndex].anchoredPosition = licalPosTiles2[tileIndex];

            // Reparent back to the new tile
            tiles[tileIndex].SetParent(parentTiles[tileIndex]);
            tiles[tileIndex].transform.localPosition = Vector3.zero;

            // Set the tile child object
            parentTiles[tileIndex].GetComponent<Tile>().InitializeTileImage();
        }
    }
    
    //Spawn tiles on top of the board befre animating 
    void SpawnTilesOnTop()
    {
        for (int column = 0; column < columnsCount; column++)
        {
            List<GameObject> tilePieces = new List<GameObject>();
            List<Tile> tileParents = new List<Tile>();
            if (matches[column] == 0)
                continue;

            int firstRowIndex = rowsCount - matches[column];

            for(int row = firstRowIndex; row < rowsCount; row++)
            {
                Vector2 tileWorldPos = WorldToLocalInRect(board[row, column].transform.position, animationLayer, canvas);
                Vector2 spawnPos = new Vector2(tileWorldPos.x, tileWorldPos.y + yDistanceBetweenTwoTiles * matches[column]);
                int randomGameObjectIndex = UnityEngine.Random.Range(0, tileObjects.Length);
                GameObject piece = Instantiate(tileObjects[randomGameObjectIndex], animationLayer);
                piece.GetComponent<RectTransform>().anchoredPosition = spawnPos;

                tilePieces.Add(piece);
                tileParents.Add(board[row, column]);
            }

            StartCoroutine(AnimateSpawnedPieces(tilePieces, tileParents, duration / matches[column]));
        }
    }
    
    //Reset the state of the board
    void ResetBoard()
    {
        for(int row = 0; row < rowsCount; row++)
        {
            for (int column = 0; column < columnsCount; column++)
            {
                board[row, column].IsMatched = false;
            }
        }
    }
}
