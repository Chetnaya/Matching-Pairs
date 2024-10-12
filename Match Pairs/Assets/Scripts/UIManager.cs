using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject exitButton; 
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject musicOnButton;
    [SerializeField] private GameObject musicOffButton; 

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

        // Ensure buttons are correctly set based on the initial state of background music
        if (AudioPlayer.Instance.BackgroundMusic.isPlaying)
        {
            SetMusicOnState(true);
        }
        else
        {
            SetMusicOnState(false);
        }
    }
    // Method to handle the Music On button click
    public void OnMusicOnButtonClick()
    {
        SetMusicOnState(false); // Switch to Music Off state
        AudioPlayer.Instance.BackgroundMusic.mute = true; // Mute music
    }

    // Method to handle the Music Off button click
    public void OnMusicOffButtonClick()
    {
        SetMusicOnState(true); // Switch to Music On state
        AudioPlayer.Instance.BackgroundMusic.mute = false; // Unmute music
    }

    // Helper method to toggle between Music On and Off states
    private void SetMusicOnState(bool isOn)
    {
        musicOnButton.SetActive(isOn);
        musicOffButton.SetActive(!isOn);
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
