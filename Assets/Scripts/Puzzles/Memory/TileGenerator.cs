using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // For Raycast

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private int numTiles = 4; // Number of tiles per row/column (total will be numTiles * numTiles)
    [SerializeField] private float spacingBetweenTiles = 10f;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private float dealyBeforeReset = 0.2f;
    [SerializeField] private GameObject parentPrefab;
    public GameObject completeText;
    public PlayerController player;
    [SerializeField] private GameObject tileCanvas;  // Separate canvas for tiles
    private int correctPairs = 0;

    private bool isBusy = false;
    private Queue<Tile> selectedTiles = new Queue<Tile>();
    private RectTransform tileCanvasRectTransform;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    private void Start()
    {
        if (tileCanvas != null)
        {
            tileCanvasRectTransform = tileCanvas.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Canvas for tiles is not assigned!");
            return;
        }

        graphicRaycaster = tileCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = tileCanvas.GetComponentInParent<EventSystem>();
        GenerateTiles();
    }

    private void GenerateTiles()
    {
        if (tileCanvasRectTransform == null)
        {
            Debug.LogError("Oops! RectTransform not found.");
            return;
        }

        int totalTiles = numTiles * numTiles;

        if (totalTiles % 2 != 0)
        {
            Debug.LogError("Hmmm... total tiles must be even to form pairs!!");
            return;
        }

        Color[] tileColors = GeneratePairedColors(totalTiles / 2);
        ShuffleList(tileColors);

        Vector2 canvasSize = tileCanvasRectTransform.rect.size;

        float totalSpacingX = (numTiles - 1) * spacingBetweenTiles;
        float totalSpacingY = (numTiles - 1) * spacingBetweenTiles;

        float tileWidth = (canvasSize.x - totalSpacingX) / numTiles;
        float tileHeight = (canvasSize.y - totalSpacingY) / numTiles;

        float startX = -(canvasSize.x / 2) + (tileWidth / 2);
        float startY = (canvasSize.y / 2) - (tileHeight / 2);

        float y = startY;
        int tileIndex = 0;

        for (int i = 0; i < numTiles; i++)
        {
            float x = startX;
            for (int j = 0; j < numTiles; j++)
            {
                Tile tile = Instantiate<Tile>(tilePrefab, tileCanvas.transform);  // Instantiate on the separate canvas
                tile.Init(tileColors[tileIndex]);

                RectTransform tileRectTransform = tile.GetComponent<RectTransform>();
                tileRectTransform.anchoredPosition = new Vector2(x, y);
                tileRectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);

                x += tileWidth + spacingBetweenTiles;
                tileIndex++;
            }
            y -= tileHeight + spacingBetweenTiles;
        }
    }

    private Color[] GeneratePairedColors(int pairCount)
    {
        Color[] colors = new Color[pairCount * 2];
        float hueStep = 1.0f / pairCount;

        for (int i = 0; i < pairCount; i++)
        {
            float hue = i * hueStep;
            Color generatedColor = Color.HSVToRGB(hue, 0.8f, 0.9f);

            // Assign the same color twice to form a pair
            colors[i * 2] = generatedColor;
            colors[i * 2 + 1] = generatedColor;
        }

        return colors;
    }

    private void ShuffleList(Color[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            Color temp = list[i];
            int randomIndex = Random.Range(i, list.Length);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Update method to detect mouse click on the canvas
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isBusy)
        {
            // Raycast to detect which UI element is clicked
            PointerEventData pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results)
            {

                    if (result.gameObject.CompareTag("Panel"))
                    {
                        break;
                    }
                Debug.Log("Clicked on: " + result.gameObject.name);
                Tile clickedTile = result.gameObject.GetComponent<Tile>();
                if (clickedTile != null)
                {
                    bool queue = true;
                    if (selectedTiles.Count >= 2)
                    {
                        selectedTiles.Clear();
                    }else if (selectedTiles.Count == 1)
                    {
                        // we will also check if the clicked tile is NOT the same as the selected tile
                        if (selectedTiles.Peek().GetColor() != clickedTile.GetColor() && selectedTiles.Peek() != clickedTile){
                            // This should reset the color to the default color
                            StartCoroutine(ResetTiles(clickedTile));
                            queue = false;
                        }else{
                            // Got it right !! 
                            queue = false;
                            correctPairs++;
                            if (correctPairs >= (numTiles * numTiles / 2)){
                                Debug.Log("All Paired!");
                                StartCoroutine(ShowCompleteTextAndDisableSystem());
                            }
                            StartCoroutine(DestroyTiles(clickedTile));
                        }
                    }
                    if (queue)
                    {
                        clickedTile.OnTileClicked();
                        selectedTiles.Enqueue(clickedTile);
                    }
                    // We don't need to continue checking the other UI elements because we found the clicked tile !! 
                    break;
                }
            }
        }
    }

    private IEnumerator DestroyTiles(Tile tile)
    {
        this.isBusy = true;
        tile.OnTileClicked();
        yield return new WaitForSecondsRealtime(dealyBeforeReset);
        Destroy(tile.gameObject);
        Destroy(selectedTiles.Dequeue().gameObject);
        this.isBusy = false;
    }
    private IEnumerator ResetTiles(Tile tile)
    {
        this.isBusy = true;
        tile.OnTileClicked();
        yield return new WaitForSecondsRealtime(dealyBeforeReset);
        tile.Reset();
        selectedTiles.Dequeue().Reset();
        this.isBusy = false;
    }
    public void RestartGame()
    {
        selectedTiles.Clear();

        correctPairs = 0;

        foreach (Transform child in tileCanvas.transform)
        {
            Destroy(child.gameObject);
        }
        GenerateTiles();

        isBusy = false;

    
        Debug.Log("Game Restarted!");
    }
    public void ExitGame()
    {
        player.RecoverSpeed();
        RestartGame();
    }
    public bool IsCompleted()
    {
        if (correctPairs >= (numTiles * numTiles / 2))
        {
            return true;
        }
        return false;
    }
    
    IEnumerator ShowCompleteTextAndDisableSystem()
    {
        completeText.gameObject.SetActive(true); // Show the popup

        yield return new WaitForSeconds(2);  // Wait for 2 seconds

        completeText.gameObject.SetActive(false); // Hide the popup


        ExitGame();
        this.gameObject.SetActive(false);
    }

}
