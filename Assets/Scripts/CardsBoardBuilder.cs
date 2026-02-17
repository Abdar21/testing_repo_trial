using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsBoardBuilder : MonoBehaviour
{
    [Header("Board (must have GridLayoutGroup)")]
    [SerializeField] private GridLayoutGroup grid;

    [Header("Card Prefab (UI)")]
    [SerializeField] private GameObject cardPrefab;

    [Header("Layout")]
    [Min(1)][SerializeField] private int rows = 4;
    [Min(1)][SerializeField] private int columns = 4;

    [Header("Sizing")]
    [SerializeField] private bool keepSquareCells = true;
    [SerializeField] private bool rebuildOnStart = true;

    [Header("Cards Sprites")]
    [SerializeField] private Sprite[] cardFaceSprites;

    private RectTransform _boardRect;

    private void Awake()
    {
        if (grid == null) grid = GetComponent<GridLayoutGroup>();
        _boardRect = grid != null ? grid.GetComponent<RectTransform>() : null;
    }

    private void OnEnable()
    {
        if (rebuildOnStart)
            Build(rows, columns);
    }

    public List<Card> Build(int newRows, int newColumns)
    {
        rows = Mathf.Max(1, newRows);
        columns = Mathf.Max(1, newColumns);

        int totalCards = rows * columns;

        List<Card> cards = new List<Card>();

        if (totalCards % 2 != 0)
        {
            Debug.LogError("Memory game requires even number of cards.");
            return null;
        }

        ClearChildren();

        ApplyGridConstraint(columns);
        ResizeCellsToFit(rows, columns);

        // 1️⃣ Generate paired indices
        int pairCount = totalCards / 2;
        int[] indices = GenerateShuffledPairs(pairCount);

        // 2️⃣ Instantiate cards and assign index
        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, grid.transform);
            Card card = cardObj.GetComponent<Card>();
            card.SetIndex(indices[i], i,cardFaceSprites[indices[i]]);
            cards.Add(card);
        }
        return cards;
    }

    private int[] GenerateShuffledPairs(int pairCount)
    {
        int totalCards = pairCount * 2;
        int[] indices = new int[totalCards];

        // Create pairs
        int index = 0;
        for (int i = 0; i < pairCount; i++)
        {
            indices[index++] = i;
            indices[index++] = i;
        }

        // Shuffle (Fisher-Yates)
        for (int i = 0; i < indices.Length; i++)
        {
            int randomIndex = Random.Range(i, indices.Length);
            int temp = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        return indices;
    }

    private void ApplyGridConstraint(int fixedColumns)
    {
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = fixedColumns;
    }

    private void ResizeCellsToFit(int r, int c)
    {
        Rect rect = _boardRect.rect;

        float paddingX = grid.padding.left + grid.padding.right;
        float paddingY = grid.padding.top + grid.padding.bottom;

        float spacingX = grid.spacing.x * (c - 1);
        float spacingY = grid.spacing.y * (r - 1);

        float availableW = rect.width - paddingX - spacingX;
        float availableH = rect.height - paddingY - spacingY;

        float cellW = availableW / c;
        float cellH = availableH / r;

        if (keepSquareCells)
        {
            float size = Mathf.Floor(Mathf.Min(cellW, cellH));
            grid.cellSize = new Vector2(size, size);
        }
        else
        {
            grid.cellSize = new Vector2(Mathf.Floor(cellW), Mathf.Floor(cellH));
        }
    }

    private void ClearChildren()
    {
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = grid.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void SetLayoutAndRebuild(int newRows, int newColumns)
    {
        Build(newRows, newColumns);
    }
}
