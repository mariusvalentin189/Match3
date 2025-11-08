using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class Game : MonoBehaviour
{
    public static Game Instance;

    private void Awake()
    {
        Instance = this;

    }
    [SerializeField] int rowsCount;
    [SerializeField] int columnsCount;
    [SerializeField] GameObject[] tileObjects; // pieces
    [SerializeField] Color selectedTileColor; //Tile color for when selected
    [SerializeField] Color deselectedTileColor; //Tile color for when deselected
    [SerializeField] Tile tilePrefab; // Tile to spawn prefab
    [SerializeField] RectTransform animationLayer; // Empty GameObject under Canvas (with no layout groups)
    [SerializeField] float tilesSpeed = 5f; //Tiles speed
    [SerializeField] float yDistanceBetweenTwoTiles = 60; //y distance in between two tiles (grid layout spacing + tiles size)
    [SerializeField] int baseScore; //score per each piece
    [SerializeField] TMP_Text scoreText;
    [SerializeField] int numberOfMoves; //max moves
    [SerializeField] TMP_Text movesText;
    [SerializeField] float timwWithNoMatches; //countdown timer after which the tile that can match is highlighted
    [SerializeField] float newMatchDelayTime; //delay to do consecutive matches (new matches after tiles fall)
    [SerializeField] float boardShuffleDelay; //delay to spawn new tiles after the old ones are removed
    [SerializeField] float highlightedTileSwitchStateTime;

    float currentTimer = 0;
    int currentScore;
    bool bringingPiecesDown = false;
    int numOfRowsAnimated = 0;
    Tile[,] board;
    Tile selectedTile;
    Tile secondTile;
    bool canMove = true;
    int[] matches;
    Canvas canvas;
    bool sugestedTile = false;
    Tile suggestedTileObject;
    bool highlightedTile = false;
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        matches = new int[columnsCount];
        board = new Tile[rowsCount, columnsCount];
        SpawnGrid();
        UpdateScore();
        UpdateMovesLeft();
    }
    private void Update()
    {
        // if no tile is selected and no move/matching is being made start a counter and highlight a tile that can make a match
        // If no tile that can do a match is found shuffle the board
        if (selectedTile == null && canMove)
        {
            if (!sugestedTile)
            {
                suggestedTileObject = CheckPossibleMatches();
                sugestedTile = true;
            }
            else if (suggestedTileObject != null)
            {
                if (currentTimer < timwWithNoMatches)
                {
                    currentTimer += Time.deltaTime;
                }
                else
                {
                    //Highlight Tile
                    if (!highlightedTile)
                    {
                        StartCoroutine(HighlightTile(suggestedTileObject));
                    }
                }
            }
            else
            {
                //Swap board
                StartCoroutine(RespawnPieces());
            }
        }
    }
    //Spawn the grid where the pieces will be placed  
    void SpawnGrid()
    {
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                board[i,j] = Instantiate(tilePrefab, transform);
                board[i,j].GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;
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

    //Respawn new piecces when there are no other moves to be made
    IEnumerator RespawnPieces()
    {
        canMove = false;
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                Destroy(board[i, j].TileImageObject);
                board[i, j].InitializeTileImage();
            }
        }
        yield return new WaitForSeconds(boardShuffleDelay);
        SpawnPieces();
        currentTimer = 0;
        sugestedTile = false;
        highlightedTile = false;
        canMove = true;
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

        sugestedTile = false;

        currentTimer = 0;

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

                    //Handle destroying the mathed pieces
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

    //Check the newly spawned pieces after new tiles spawn
    bool CheckMatchingTilesAfterTilesSpawn(Tile tile)
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

    //Check if any match is found after making a move (creating a swap and checking if any new tiles swap)
    //Used to suggest a nrea match in case the player does not find any
    bool CheckNewMatchesAfterMatching(Tile tile)
    {
        int objectType = tile.TileImageId;
        int rowIndex = tile.XIndex;
        int columnIndex = tile.YIndex;
        if (rowIndex >= 2)
        {
            if (board[rowIndex - 1, columnIndex].TileImageId == objectType && board[rowIndex - 2, columnIndex].TileImageId == objectType)
            {
                return true;
            }
        }
        if (rowIndex <= rowsCount - 3)
        {
            if (board[rowIndex + 1, columnIndex].TileImageId == objectType && board[rowIndex + 2, columnIndex].TileImageId == objectType)
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
        if (columnIndex <= columnsCount - 3)
        {
            if (board[rowIndex, columnIndex + 1].TileImageId == objectType && board[rowIndex, columnIndex + 2].TileImageId == objectType)
            {
                return true;
            }
        }
        //Column three match case but in between two tiles
        if (rowIndex >= 1 && rowIndex <= rowsCount - 2)
        {
            if (board[rowIndex - 1, columnIndex].TileImageId == objectType && board[rowIndex + 1, columnIndex].TileImageId == objectType)
            {
                return true;
            }
        }
        //Row three match case but in between two tiles
        if (columnIndex >= 1 && columnIndex <= columnsCount - 2)
        {
            if (board[rowIndex, columnIndex - 1].TileImageId == objectType && board[rowIndex, columnIndex + 1].TileImageId == objectType)
            {
                return true;
            }
        }
        return false;
    }

    //Check if the two pieces selected can match
    bool CheckSelectedTilesMatching()
    {
        bool found = false;

        //Temporary swap tiles colors
        TemporarySwapTiles(selectedTile, secondTile);

        if (CheckMatchingTilesBeforeSwap(selectedTile))
        {
            found = true;
        }

        if (CheckMatchingTilesBeforeSwap(secondTile))
        {
            found = true;
        }

        //Swap tiles back
        TemporarySwapTiles(selectedTile, secondTile);
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

        while (Vector2.Distance(tile1.anchoredPosition, localPosTile2) > 0.01f)
        {
            tile1.anchoredPosition = Vector2.MoveTowards(tile1.anchoredPosition, localPosTile2, tilesSpeed * Time.deltaTime);
            tile2.anchoredPosition = Vector2.MoveTowards(tile2.anchoredPosition, localPosTile1, tilesSpeed * Time.deltaTime);
            yield return null;
        }

        numberOfMoves--;
        UpdateMovesLeft();

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
                currentScore += baseScore;
                Destroy(board[i, j].TileImageObject);
                board[i, j].InitializeTileImage();
            }
        }

        yield return null;

        UpdateScore();

        BringPiecesDown();

        while (bringingPiecesDown)
        {
            if (numOfRowsAnimated == 0)
            {
                ResetBoard();
                secondTile = null;
                selectedTile = null;
                bringingPiecesDown = false;
                matches = new int[columnsCount];
            }
            yield return null;
        }

        bool newMatchesFound;

        do
        {
            yield return new WaitForSeconds(newMatchDelayTime);
            newMatchesFound = false;
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    if (CheckMatchingTilesAfterTilesSpawn(board[i, j]))
                    {
                        newMatchesFound = true;
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
                        currentScore += baseScore;
                        Destroy(board[i, j].TileImageObject);
                        board[i, j].InitializeTileImage();
                    }
                }

                yield return null;

                UpdateScore();

                BringPiecesDown();

                while (bringingPiecesDown)
                {
                    if (numOfRowsAnimated == 0)
                    {
                        ResetBoard();
                        bringingPiecesDown = false;
                        matches = new int[columnsCount];
                    }
                    yield return null;
                }
            }
            else
            {
                canMove = true;

                //Check if all the moves were used and end the level
                if (numberOfMoves == 0)
                {
                    PauseMenu.Instance.FinishLevel(currentScore);
                }
            }
        } while (newMatchesFound);
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
        SpawnTilesOnTop();
        for (int column = 0; column < columnsCount; column++)
        {
            if (matches[column] == 0)
                continue;

            int matchedFirstIndex = -1;
            int matchedPositionsOffest = -1;
            bool foundFirstMatchedPiece = false;
            int matchesUntilPieceFound = 0;
            int row = 0;
            while (row < rowsCount)
            {
                int lastIndex = 0;
                if (!foundFirstMatchedPiece)
                {
                    while (board[row, column].IsMatched)
                    {
                        matchesUntilPieceFound++;
                        foundFirstMatchedPiece = true;

                        //If all the tiles on top rows are matched, there are no tiles to fall down
                        //New tiles will be spawned instead
                        if (row == rowsCount - 1)
                        {
                            foundFirstMatchedPiece = false;
                            break;
                        }
                        row++;

                    }
                    if (foundFirstMatchedPiece)
                    {
                        matchedFirstIndex = row;
                        if (row == rowsCount - 1)
                        {
                            lastIndex = row + 1;
                            foundFirstMatchedPiece = false;
                            matchedPositionsOffest = matchesUntilPieceFound;
                            matchesUntilPieceFound = 0;
                            bringingPiecesDown = true;
                            StartCoroutine(AnimatePiecesDown(matchedFirstIndex, lastIndex, column, matchedPositionsOffest));
                        }
                    }
                    else row++;
                }
                else
                {
                    if (board[row, column].IsMatched && !board[row - 1, column].IsMatched)
                    {
                        lastIndex = row;
                        foundFirstMatchedPiece = false;
                        matchedPositionsOffest = matchesUntilPieceFound;
                        bringingPiecesDown = true;
                        StartCoroutine(AnimatePiecesDown(matchedFirstIndex, lastIndex, column, matchedPositionsOffest));
                    }
                    else if (row == rowsCount - 1)
                    {
                        lastIndex = rowsCount;
                        foundFirstMatchedPiece = false;
                        matchedPositionsOffest = matchesUntilPieceFound;
                        matchesUntilPieceFound = 0;
                        bringingPiecesDown = true;
                        StartCoroutine(AnimatePiecesDown(matchedFirstIndex, lastIndex, column, matchedPositionsOffest));
                    }
                    else row++;
                }
            }
        }
    }
    
    //Animate the movement of pieces comming down
    IEnumerator AnimatePiecesDown(int matchedFirstIndex, int matchedLastIndex, int column, int matchedPositionsOffest)
    {
        numOfRowsAnimated++;
        List<Vector2> licalPosTiles1 = new List<Vector2>();
        List<Vector2> licalPosTiles2 = new List<Vector2>();
        List<RectTransform> tiles = new List<RectTransform>();
        List<Transform> parentTiles = new List<Transform>();
        int numOfPieces = rowsCount - matchedFirstIndex;
        for (int row = matchedFirstIndex; row < matchedLastIndex; row++)
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
        while (numOfPieces > 0) 
        {
            for (int tileIndex = 0; tileIndex < licalPosTiles1.Count(); tileIndex++)
            {
                if (Vector2.Distance(tiles[tileIndex].anchoredPosition, licalPosTiles2[tileIndex]) > 0.01f)
                {

                    tiles[tileIndex].anchoredPosition = Vector2.MoveTowards(tiles[tileIndex].anchoredPosition, licalPosTiles2[tileIndex], tilesSpeed * 1.5f * Time.deltaTime);
                }
                else numOfPieces--;
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
        numOfRowsAnimated--;
    }

    //Animate the spawned pieces to fall
    IEnumerator AnimateSpawnedPieces(List<GameObject> tilePieces, List<Tile> tileParents)
    {
        numOfRowsAnimated++;
        List<Vector2> licalPosTiles1 = new List<Vector2>();
        List<Vector2> licalPosTiles2 = new List<Vector2>();
        List<RectTransform> tiles = new List<RectTransform>();
        List<Transform> parentTiles = new List<Transform>();
        bringingPiecesDown = true;
        int numOfTiles = tilePieces.Count();
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

        while (numOfTiles > 0)
        {
            for (int tileIndex = 0; tileIndex < licalPosTiles1.Count(); tileIndex++)
            {
                if (Vector2.Distance(tiles[tileIndex].anchoredPosition, licalPosTiles2[tileIndex]) > 0.01f)
                {
                    tiles[tileIndex].anchoredPosition = Vector2.MoveTowards(tiles[tileIndex].anchoredPosition, licalPosTiles2[tileIndex], tilesSpeed * 1.5f * Time.deltaTime);
                }
                else numOfTiles--;
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
        numOfRowsAnimated--;
    }
    
    //Spawn tiles on top of the board befre animating 
    void SpawnTilesOnTop()
    {
        List<GameObject> tilePieces = new List<GameObject>();
        List<Tile> tileParents = new List<Tile>();
        for (int column = 0; column < columnsCount; column++)
        {
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

        }
        StartCoroutine(AnimateSpawnedPieces(tilePieces, tileParents));
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

    //Update the score text
    void UpdateScore()
    {
        scoreText.text = "SCORE: " + currentScore.ToString();

    }

    //Update the moves left text
    void UpdateMovesLeft()
    {
        movesText.text = "MOVES LEFT: " + numberOfMoves.ToString(); 
    }

    //Check if any possible match can be made and return the tile that can do the match
    Tile CheckPossibleMatches()
    {
        var directions = new (int dr, int dc)[] { (0, 1), (1, 0) };

        for (int row = 0; row < rowsCount; row++)
        {
            for (int column = 0; column < columnsCount; column++)
            {
                foreach (var (dr, dc) in directions)
                {
                    int newRow = row + dr;
                    int newCol = column + dc;

                    if (newRow >= rowsCount || newCol >= columnsCount)
                        continue;

                    TemporarySwapTiles(board[row, column], board[newRow, newCol]);

                    if (CheckNewMatchesAfterMatching(board[row, column]))
                    {
                        //Swap tiles back
                        TemporarySwapTiles(board[row, column], board[newRow, newCol]);
                        return board[newRow, newCol];
                    }
                    if (CheckNewMatchesAfterMatching(board[newRow, newCol]))
                    {
                        //Swap tiles back
                        TemporarySwapTiles(board[row, column], board[newRow, newCol]);
                        return board[row, column];
                    }

                    //Swap tiles back
                    TemporarySwapTiles(board[row, column], board[newRow, newCol]);
                }
            }
        }
        return null;
    }

    //Highlight the tile that can do a match
    IEnumerator HighlightTile (Tile tile)
    {
        highlightedTile = true;
        bool selected = false;
        while(selectedTile == null)
        {
            selected = !selected;
            if (selected)
            {
                tile.GetComponent<UnityEngine.UI.Image>().color = selectedTileColor;
            }
            else
            {
                tile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;
            }
            if (selectedTile == null)
            {
                yield return new WaitForSeconds(highlightedTileSwitchStateTime);
            }
        }
        if (tile != selectedTile)
        {
            tile.GetComponent<UnityEngine.UI.Image>().color = deselectedTileColor;
        }
        sugestedTile = false;
        highlightedTile = false;
        currentTimer = 0;

    }

    //Temporary swap tiles pieces
    void TemporarySwapTiles(Tile firstTile, Tile secondTile)
    {
        TileImage tempImg = firstTile.TileImageObject.GetComponent<TileImage>();
        firstTile.SetTileImageTemp(secondTile.TileImageObject.GetComponent<TileImage>());
        secondTile.SetTileImageTemp(tempImg);
    }
}
