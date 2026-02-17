using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardListIndex;
    public int cardIndex { get; private set; }
    public bool isFlipped=false;

    public void SetIndex(int index,int listIndex, Sprite sp)
    {
        cardIndex = index;
        cardListIndex = listIndex;
        transform.GetChild(1).GetComponent<Image>().sprite = sp;
    }

    public void OnCardClick()
    {
        if (isFlipped)
            return;

        ShowCard();
        MemoryGameController.Instance.OnCardFlip(cardListIndex);
    }
    public void ShowCard()
    {
        isFlipped = true;
        transform.DORotate(new Vector3(0, 90, 0), 0.35f).OnComplete(() =>
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.DORotate(new Vector3(0, 0, 0), 0.35f).OnComplete(() =>
            {
                //
            });
        });
    }
    public void HideCard()
    {
        isFlipped = false;
        transform.DORotate(new Vector3(0, 90, 0), 0.35f).OnComplete(() =>
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.DORotate(new Vector3(0, 0, 0), 0.35f);
        });
    }
    public void MatchedCard()
    {
        transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.3f).OnComplete(() =>
        {
            transform.DOScale(new Vector3(1, 1, 1), 0.3f);
        });
    }
    public void EnableButton()
    {
        GetComponent<Button>().enabled = true;
    }
    public void DisableButton()
    {
        GetComponent<Button>().enabled = false;
    }

    
}
