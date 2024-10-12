using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject exitButton; 
    [SerializeField] private GameObject instructionPanel;

    void Start()
    {
        // Check the platform and toggle the exit button accordingly
#if UNITY_STANDALONE_WIN
            exitButton.SetActive(true);
#else
        exitButton.SetActive(false);
#endif

        // Initially hide the instruction panel
        instructionPanel.SetActive(false);
    }

    // Call this method when the exit button is clicked
    public void OnExitButtonClick()
    {
#if UNITY_STANDALONE_WIN
            Application.Quit();
#endif
    }

    // Call this method when the instructions button is clicked
    public void OnInstructionsButtonClick()
    {
        instructionPanel.SetActive(true); // Show the instructions panel
    }

    // Call this method when the back button on the instruction panel is clicked
    public void OnInstructionsBackBtnClick()
    {
        _CardGameManager.Instance.InterruptCountdown();
    }

    // Skip the countdown and start the game immediately
    public void OnSkipButtonClick()
    {
        _CardGameManager.Instance.SkipCountdownAndStartGame();
    }
}
