using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGameController : MonoBehaviour
{
    public static MemoryGameController Instance { get; private set; }

    [Header("Cards Board Builder")]
    public CardsBoardBuilder boardBuilder;
    [Header("Layout")]
    [Min(1)][SerializeField] private int rows = 4;
    [Min(1)][SerializeField] private int columns = 4;

    [Header("Statictis")]
    public int movesCount=0;
    public int matches=0;
    public int firstSelected=-1;
    public int secondSelected=-1;
    public int allMatchesNum= 0;
    public int score= 0;
    public int scoreCombo= 1;

    public List<Card> cards;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void OnEnable()
    {
        PrepareGame();
    }
    public void PrepareGame()
    {
        cards = new List<Card>();
        cards = boardBuilder.Build(rows, columns);

        movesCount = 0;
        matches = 0;
        allMatchesNum = (rows * columns) / 2;

        firstSelected = -1;
        secondSelected = -1;

        score = 0;
        scoreCombo = 1;
        StartGame();
    }
    public void StartGame()
    {
        StartCoroutine(StartGameDelay());
    }


    public void OnCardFlip(int index)
    {
        if (firstSelected == -1)
        {
            firstSelected = index;
            secondSelected = -1;
        }
        else if (secondSelected == -1)
        {
            secondSelected = index;
            if (cards[firstSelected].cardIndex == cards[secondSelected].cardIndex)
            {
                StartCoroutine(CheckCards(true, firstSelected, secondSelected));
            }
            else
            {
                StartCoroutine(CheckCards(false, firstSelected,secondSelected));
            }
            firstSelected = -1;
            
        }
    }
    private IEnumerator StartGameDelay()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < cards.Count; i++) 
        {

            cards[i].DisableButton();
            cards[i].ShowCard();
        }
        yield return new WaitForSeconds(2.5f);
        for (int i = 0; i < cards.Count; i++) 
        {
            cards[i].HideCard();
            cards[i].EnableButton();
        }

    }
    private IEnumerator CheckCards(bool status, int f, int s)
    {
        yield return new WaitForSeconds(1f);

        movesCount++;

        if (status)
        {
            cards[f].MatchedCard();
            cards[s].MatchedCard();
            matches++;
            if (matches == allMatchesNum)
            {
                GamesOver();
            }
        }
        else
        {
            cards[f].HideCard();
            cards[s].HideCard();
        }
    }


    void GamesOver()
    {
        
    }
    void Update()
    {
        
    }
}
