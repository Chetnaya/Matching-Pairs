﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Sentences
{
    public List<string> sentences;
}
public class _CardGameManager : MonoBehaviour
{

    public static _CardGameManager Instance;
    public static int gameSize = 2;
    // gameobject instance
    [SerializeField]
    private GameObject prefab;
    // parent object of cards
    [SerializeField]
    private GameObject cardList;
    // sprite for card back
    [SerializeField]
    private Sprite cardBack;
    // all possible sprite for card front
    [SerializeField]
    private Sprite[] sprites;
    // list of card
    private _Card[] cards;

    //we place card on this panel
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject info;

    [SerializeField]
    private GameObject infoPanel;

    // for preloading
    [SerializeField]
    private _Card spritePreload;
    // other UI
    [SerializeField]
    private Text sizeLabel;
    [SerializeField]
    private Slider sizeSlider;
    [SerializeField]
    private Text timeLabel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI gameStartingInfoText;
    [SerializeField] private TextMeshProUGUI encouragingSentenceText;

    public GameObject skipButton;

    [SerializeField]
    /*private tmppro timeLabel;*/

    private float time;
    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private bool gameStart;
    private bool countdownInterrupted;

    private Coroutine countdownCoroutine;

    [SerializeField]
    private GameObject Mainmenu;


    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gameStart = false;
        countdownInterrupted = false;
        panel.SetActive(false);
        ResetUIElements();
    }

    private void ResetUIElements()
    {
        timerText.gameObject.SetActive(false);
        gameStartingInfoText.gameObject.SetActive(false);
        skipButton.SetActive(false);
        timerText.text = "5"; // Reset timer text to 5
    }

    public void StartCardGame()
    {
        if (gameStart) return;

        // If a countdown coroutine is already running, stop it
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        // Start the new countdown coroutine
        countdownCoroutine = StartCoroutine(ShowInstructionsBeforeGame());
    }

    private IEnumerator ShowInstructionsBeforeGame()
    {
        countdownInterrupted = false;

        infoPanel.SetActive(true);
        gameStartingInfoText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        skipButton.SetActive(true);

        for (int i = 5; i > 0; i--)
        {
            if (countdownInterrupted)
            {
                ResetUIElements();
                yield break; // Exit the coroutine if interrupted
            }

            timerText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }

        if (countdownInterrupted) yield break;

        infoPanel.SetActive(false);
        gameStartingInfoText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        skipButton.SetActive(false);

        StartGame();
    }

    private void StartGame()
    {
        gameStart = true;
        panel.SetActive(true);
        SetGamePanel();
        cardSelected = spriteSelected = -1;
        cardLeft = cards.Length;
        SpriteCardAllocation();
        StartCoroutine(HideFace());
        time = 0;
        Mainmenu.SetActive(false);
        DisplayRandomEncouragingSentence();
    }

    public void InterruptCountdown()
    {
        countdownInterrupted = true;
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine); // Stop the coroutine if running
        }
        ResetUIElements();
        infoPanel.SetActive(false);
    }
    // This method is called when the skip button is clicked
    public void SkipCountdownAndStartGame()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine); // Stop the countdown
        }

        HideCountdownUI();
        StartGame();
    }
    // Helper method to hide countdown-related UI elements
    private void HideCountdownUI()
    {
        infoPanel.SetActive(false);
        gameStartingInfoText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        skipButton.SetActive(false);
    }

    private void DisplayRandomEncouragingSentence()
    {
        string json = Resources.Load<TextAsset>("encouraging_sentences").text;
        Sentences sentences = JsonUtility.FromJson<Sentences>(json);

        // Select a random sentence from the list
        string randomSentence = sentences.sentences[Random.Range(0, sentences.sentences.Count)];

        // Display the sentence in the UI
        encouragingSentenceText.text = randomSentence;
    }

    private void PreloadCardImage()
    {
        for (int i = 0; i < sprites.Length; i++)
            spritePreload.SpriteID = i;
        spritePreload.gameObject.SetActive(false);
    }


    // Initialize cards, size, and position based on size of game
    private void SetGamePanel(){
        // if game is odd, we should have 1 card less
        int isOdd = gameSize % 2 ;

        cards = new _Card[gameSize * gameSize - isOdd];
        // remove all gameobject from parent
        foreach (Transform child in cardList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        // calculate position between each card & start position of each card based on the Panel
        RectTransform panelsize = panel.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        float row_size = panelsize.sizeDelta.x;
        float col_size = panelsize.sizeDelta.y;
        float scale = 1.0f/gameSize;
        float xInc = row_size/gameSize;
        float yInc = col_size/gameSize;
        float curX = -xInc * (float)(gameSize / 2);
        float curY = -yInc * (float)(gameSize / 2);

        if(isOdd == 0) {
            curX += xInc / 2;
            curY += yInc / 2;
        }
        float initialX = curX;
        // for each in y-axis
        for (int i = 0; i < gameSize; i++)
        {
            curX = initialX;
            // for each in x-axis
            for (int j = 0; j < gameSize; j++)
            {
                GameObject c;
                // if is the last card and game is odd, we instead move the middle card on the panel to last spot
                if (isOdd == 1 && i == (gameSize - 1) && j == (gameSize - 1))
                {
                    int index = gameSize / 2 * gameSize + gameSize / 2;
                    c = cards[index].gameObject;
                }
                else
                {
                    // create card prefab
                    c = Instantiate(prefab);
                    // assign parent
                    c.transform.parent = cardList.transform;

                    int index = i * gameSize + j;
                    cards[index] = c.GetComponent<_Card>();
                    cards[index].ID = index;
                    // modify its size
                    c.transform.localScale = new Vector3(scale, scale);
                }
                // assign location
                c.transform.localPosition = new Vector3(curX, curY, 0);
                curX += xInc;

            }
            curY += yInc;
        }

    }
    // reset face-down rotation of all cards
    void ResetFace()
    {
        for (int i = 0; i < gameSize; i++)
            cards[i].ResetRotation();
    }
    // Flip all cards after a short period
    IEnumerator HideFace()
    {
        //display for a short moment before flipping
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();
        yield return new WaitForSeconds(0.5f);
    }
    // Allocate pairs of sprite to card instances
    private void SpriteCardAllocation()
    {
        int i, j;
        int[] selectedID = new int[cards.Length / 2];
        // sprite selection
        for (i = 0; i < cards.Length/2; i++)
        {
            // get a random sprite
            int value = Random.Range(0, sprites.Length - 1);
            // check previous number has not been selection
            // if the number of cards is larger than number of sprites, it will reuse some sprites
            for (j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }
            selectedID[i] = value;
        }

        // card sprite deallocation
        for (i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }
        // card sprite pairing allocation
        for (i = 0; i < cards.Length / 2; i++)
            for (j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length - 1);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }

    }
    // Slider update gameSize
    public void SetGameSize() {
        gameSize = (int)sizeSlider.value;
        sizeLabel.text = gameSize + " X " + gameSize;
    }
    // return Sprite based on its id
    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }
    // return card back Sprite
    public Sprite CardBack()
    {
        return cardBack;
    }
    // check if clickable
    public bool canClick()
    {
        if (!gameStart)
            return false;
        return true;
    }
    // card onclick event
    public void cardClicked(int spriteId, int cardId)
    {
        // first card selected
        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
        }
        else
        { // second card selected
            if (spriteSelected == spriteId)
            {
                //correctly matched
                cards[cardSelected].Inactive();
                cards[cardId].Inactive();
                cardLeft -= 2;
                CheckGameWin();
            }
            else
            {
                // incorrectly matched
                cards[cardSelected].Flip();
                cards[cardId].Flip();
            }
            cardSelected = spriteSelected = -1;
        }
    }
    // check if game is completed
    private void CheckGameWin()
    {
        // win game
        if (cardLeft == 0)
        {
            EndGame();
            AudioPlayer.Instance.PlayAudio(1);
        }
    }
    // stop game
    private void EndGame()
    {
        gameStart = false;
        panel.SetActive(false);
        Mainmenu.SetActive(true);
    }
    public void GiveUp()
    {
        EndGame();
    }
    public void DisplayInfo(bool i)
    {
        info.SetActive(i);
    }
    // track elasped time
    private void Update(){
        if (gameStart) {
            time += Time.deltaTime;
            timeLabel.text = "Time: " + time + "s";
        }
    }
}
