using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] int gameTilesRowsCount = 2;
    [SerializeField] Tile[] gameTiles;
    Tile selectedTile;
    void Start()
    {
        gameTiles = GetComponentsInChildren<Tile>();
        SetTilesGameIndex();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                gameTiles[i].SetIndex(currentRow, i - currentIndex);
            }
            currentIndex = i;
            currentRow += 1;
        }
    }

    public void SelectTile(Tile tile)
    {
        if (selectedTile == null)
        {
            selectedTile = tile;
        }
        else
        {
            //Swap tiles
            //TODO: Check if a match can be made and cancel the swap if not possible
            int t = selectedTile.GetTileObjectId();
            selectedTile.SetTileObjectId(tile.GetTileObjectId());
            tile.SetTileObjectId(t);

            //Swap Images
            GameObject firstImage = selectedTile.GetTileImage();
            GameObject secondImage = tile.GetTileImage();

            firstImage.transform.SetParent(tile.transform);
            firstImage.transform.localPosition = new Vector2(0, 0);

            secondImage.transform.SetParent(selectedTile.transform);
            secondImage.transform.localPosition = new Vector2(0, 0);

            selectedTile.InitializeTileImage();
            tile.InitializeTileImage();

            selectedTile = null;
        }
    }
}
