using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGameController : MonoBehaviour
{
    public static MemoryGameController Instance { get; private set; }
    [SerializeField] private Fader[] pages;

    [Header("MainMenu")]
    [SerializeField] private TextMeshProUGUI bestScore;
    [SerializeField] private Button homeBtn;

    [Header("Cards Board Builder")]
    [SerializeField] private CardsBoardBuilder boardBuilder;

    [Header("Layout (Total Cards Number Should be Even)")]
    [Min(1)][SerializeField] private int rows = 4;
    [Min(1)][SerializeField] private int columns = 4;

    [Header("Statictis")]
    [SerializeField] private int movesCount=0;
    [SerializeField] private int matches=0;
    [SerializeField] private int allMatchesNum= 0;
    [SerializeField] private int score= 0;
    [SerializeField] private int scoreCombo= 1;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI scoreComboTxt;
    int firstSelected=-1;
    int secondSelected=-1;

    [Header("Audio")]
    [SerializeField] private AudioSource flipCard;
    [SerializeField] private AudioSource wrongMatch;
    [SerializeField] private AudioSource correctMatch;
    [SerializeField] private AudioSource winSound;


    List<Card> cards;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        OpenIdlePage();
    }

    public void OpenIdlePage()
    {
        
        OpenPageByIndex(0);
        bestScore.text = LoadBestScore("score").ToString();
    }
    public void OpenPageByIndex(int index)
    {
        for(int i = 0; i < pages.Length; i++)
        {
            if (i == index)
            {
                pages[i].gameObject.SetActive(false);
                pages[i].gameObject.SetActive(true);
            }
            else
            {
                pages[i].FadeOutAndDisable();
            }
        }
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
        PlusScore(0);
        scoreTxt.text = score.ToString();
        scoreComboTxt.gameObject.SetActive(false);

        homeBtn.interactable = true;

        StartGame();
    }
    private void StartGame()
    {
        OpenPageByIndex(1);
        StartCoroutine(StartGameDelay());
    }
    public void OnCardFlip(int index)
    {
        flipCard.Play();
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
    private void PlusScore(int value)
    {
        if(value == 0)
        {
            scoreCombo = 1;
            scoreComboTxt.GetComponent<Fader>().FadeOutAndDisable();
            return;
            
        }

        if (scoreCombo == 2)
        {
            scoreComboTxt.text = "x" + scoreCombo;
            scoreComboTxt.gameObject.SetActive(true);
            score += value;
        }
        else if(scoreCombo > 2)
        {
            scoreComboTxt.text = "x" + scoreCombo;
            score += value * (scoreCombo-1);
        }
        else
        {
            score += value;
        }

        scoreTxt.text = score.ToString();

        scoreCombo++;
    }
    void GamesOver()
    {
        StartCoroutine(GameOverDelay());
    }
    public void QuitButton()
    {
        Application.Quit();
    }


    private IEnumerator StartGameDelay()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].DisableButton();
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < cards.Count; i++) 
        {
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
            correctMatch.Play();
            matches++;
            PlusScore(5);
            if (matches == allMatchesNum)
            {
                scoreComboTxt.GetComponent<Fader>().FadeOutAndDisable();
                GamesOver();

            }
        }
        else
        {
            cards[f].HideCard();
            cards[s].HideCard();
            wrongMatch.Play();
            PlusScore(0);
        }
    }
    private IEnumerator GameOverDelay()
    {
        homeBtn.interactable = false;
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].MatchedCard();
            winSound.Play();
        }
        yield return new WaitForSeconds(2f);
        SaveBestPoints("score", score);
        OpenIdlePage();
    }


    public static void SaveBestPoints(string key, int points)
    {
        int current = PlayerPrefs.GetInt(key, 0);

        if (points > current)
        {
            PlayerPrefs.SetInt(key, points);
            PlayerPrefs.Save();
        }
    }
    public int LoadBestScore(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }
}
